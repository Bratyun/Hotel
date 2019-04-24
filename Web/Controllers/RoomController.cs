using API.DAL;
using Contract.DAL;
using Contract.Models;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Web.Models;

namespace Hotel.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        public RoomProxy RoomService =>
            new RoomProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public UsersProxy UserService =>
            new UsersProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public CheckProxy CheckService =>
            new CheckProxy(HttpContext?.Session.GetString(SessionKeys.Token));
        
        [AllowAnonymous]
        public async Task<ActionResult> List(RoomSortBy orderBy = RoomSortBy.None, bool desc = false)
        {
            IEnumerable<Room> rooms = await RoomService.List(orderBy, desc);
            
            return View(rooms);
        }

        [Authorize(Roles = "Owner, Admin")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(RoomRegisterViewModel model, IFormFile uploadImage)
        {
            if (ModelState.IsValid && uploadImage != null)
            {
                string type = uploadImage.ContentType;
                if (!type.StartsWith("image"))
                {
                    ModelState.AddModelError("Image", "It is not image");
                    return View(model);
                }

                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)uploadImage.Length);
                }
                model.Image = imageData;
                Room room = new Room(model.RoomSize, model.Comfort, model.Image, model.Price, model.Status);

                await RoomService.Add(room);
                return RedirectToAction("List");
            }
            if (uploadImage == null)
            {
                ModelState.AddModelError("", "Not valied file");
            }
            return View(model);
        }

        [Authorize(Roles = "Owner, Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            await RoomService.Delete(id);
            return RedirectToAction("List", ModelState);
        }

        [Authorize(Roles = "Owner, Admin")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            Room room = await RoomService.GetById(id);
            RoomEditViewModel model = new RoomEditViewModel();
            if (room != null)
            {
                model = new RoomEditViewModel(room);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoomEditViewModel model, IFormFile uploadImage)
        {
            Room room = await RoomService.GetById(model.Id);
            if (room == null)
            {
                ModelState.AddModelError("", "Room not found");
            }

            if (!Tools.IsDefaultDate(model.StartDate, model.EndDate) && Tools.IsInvalidDate(model.StartDate, model.EndDate))
            {
                ModelState.AddModelError("StartDate", "Long period of reserving or invalid date period");
                ModelState.AddModelError("EndDate", "Long period of reserving or invalid date period");
            }

            if (uploadImage != null)
            {
                string type = uploadImage.ContentType;
                if (!type.StartsWith("image"))
                {
                    ModelState.AddModelError("Image", "It is not image");
                }

                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)uploadImage.Length);
                }
                model.Image = imageData;
            }

            if (ModelState.IsValid)
            {
                if (model.UserId == 0 && room.UserId != 0)
                {
                    IEnumerable<Check> checks = await CheckService.List();
                    Check check = null;
                    foreach (var item in checks)
                    {
                        if (item.UserId == room.UserId && item.RoomId == room.Id )
                        {
                            check = item;
                        }
                    }
                    if (check != null)
                    {
                        check.Status = CheckStatus.Failed;
                        await CheckService.Edit(check.Id, check);
                    }
                    model.UserId = 0;
                }
                else if (model.UserId != 0)
                {
                    User user = await UserService.Get(model.UserId);
                    if (user == null || user.Id != model.UserId)
                    {
                        ModelState.AddModelError("UserId", "User not found with same id");
                        return View(model);
                    }
                    else
                    {
                        if (model.UserId != room.UserId)
                        {
                            Check check = await GetRoomByUserAndRoomId(room.UserId, room.Id);
                            if (check != null)
                            {
                                check.Status = CheckStatus.Failed;
                                await CheckService.Edit(check.Id, check);
                                Check newCheck = new Check(user.Id, model.Id, model.Price, DateTime.UtcNow, CheckStatus.New);
                                newCheck = await CheckService.Add(newCheck);
                            }
                            else if (room.UserId == 0)
                            {
                                Check newCheck = new Check(user.Id, model.Id, model.Price, DateTime.UtcNow, CheckStatus.New);
                                newCheck = await CheckService.Add(newCheck);
                            }
                        }
                        model.UserId = user.Id;
                    }
                }
                
                Room toUpdate = await RoomEditModelToRoom(model);
                await RoomService.Edit(toUpdate.Id, toUpdate);
                return RedirectToAction("List");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Reserve(int id)
        {
            Room room = await RoomService.GetById(id);
            RoomReserveViewModel model = new RoomReserveViewModel(room);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> Reserve(RoomReserveViewModel model)
        {
            if (Tools.IsInvalidDate(model.StartDate, model.EndDate))
            {
                ModelState.AddModelError("StartDate", "Long period of reserving or invalid date period");
                ModelState.AddModelError("EndDate", "Long period of reserving or invalid date period");
            }

            if (ModelState.IsValid)
            {
                Room room = await ReserveModelToRoom(model);
                room.Status = RoomStatus.Booked;
                IEnumerable<User> users = await UserService.GetList();
                User user = new User
                {
                    Id = 0
                };
                foreach (var item in users)
                {
                    if (item.Login == User.Identity.Name)
                    {
                        user = item;
                        break;
                    }
                }
                if (user.Id == 0) return View(model);
                room.UserId = user.Id;
                await RoomService.Edit(room.Id, room);
                Check check = new Check(user.Id, room.Id, room.Price, DateTime.UtcNow, CheckStatus.New);
                check = await CheckService.Add(check);
                return RedirectToAction("List");
            }

            return View(model);
        }

        private async Task<Room> ReserveModelToRoom(RoomReserveViewModel model)
        {
            Room room = await RoomService.GetById(model.Id);
            if (room == null)
            {
                room = new Room();
                room.Id = model.Id;
            }
            room.RoomSize = model.RoomSize;
            room.Comfort = model.Comfort;
            room.Price = model.Price;
            room.Image = model.Image;
            room.StartDate = model.StartDate;
            room.EndDate = model.EndDate;
            return room;
        }

        private async Task<Check> GetRoomByUserAndRoomId(int userId, int roomId)
        {
            IEnumerable<Check> checks = await CheckService.List();
            Check check = null;
            foreach (var item in checks)
            {
                if (item.UserId == userId && item.RoomId == roomId)
                {
                    check = item;
                    break;
                }
            }
            return check;
        }

        private async Task<Room> RoomEditModelToRoom(RoomEditViewModel model)
        {
            Room room = await RoomService.GetById(model.Id);
            if (room == null)
            {
                room = new Room();
                room.Id = model.Id;
            }
            room.RoomSize = model.RoomSize;
            room.Comfort = model.Comfort;
            room.Price = model.Price;
            room.Image = model.Image;
            room.StartDate = model.StartDate;
            room.EndDate = model.EndDate;
            room.UserId = model.UserId;
            room.Status = model.Status;
            return room;
        }
    }
}