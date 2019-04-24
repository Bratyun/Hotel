using API.DAL;
using Contract.DAL;
using Contract.Models;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models;

namespace Hotel.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        public RequestProxy RequestService =>
            new RequestProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public UsersProxy UserService =>
            new UsersProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public RoomProxy RoomService =>
            new RoomProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public async Task<ActionResult> List()
        {
            IEnumerable<Request> requests;
            if (User.IsInRole("Admin"))
            {
                requests = await GetRequestsForAdmin();
            }
            else
            {
                requests = await GetByUser(User.Identity.Name);
            }
            return View(RequestsToModels(requests));
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(RequestViewModel model)
        {
            if (Tools.IsInvalidDate(model.StartDate, model.EndDate))
            {
                ModelState.AddModelError("StartDate", "Long period of reserving or invalid date period");
                ModelState.AddModelError("EndDate", "Long period of reserving or invalid date period");
            }

            Logger.Debug("Start of creating model");
            Request request = new Request();
            request.RoomSize = model.RoomSize;
            request.Status = model.Status;
            request.Comfort = model.Comfort;
            request.EndDate = model.EndDate;
            request.StartDate = model.StartDate;
            ApplicationUser user = UserAccess.GetUserByName(User.Identity.Name);
            request.UserId = user?.Id;
            request.Status = RequestStatus.New;
            
            if (ModelState.IsValid)
            {
                Logger.Debug("Model valid");
                RequestAccess.Add(request);
                return RedirectToAction("List");
            }
            else
            {
                Logger.Debug("Model does not valid");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Owner")]
        public ActionResult Answer(int id)
        {
            int comfort = -1;
            int size = -1;
            Request request = RequestAccess.GetById(id);
            if (request != null)
            {
                size = request.RoomSize;
                comfort = request.Comfort;
            }
            List<Room> rooms = RoomAccess.GetRoomByComfortAndSize(comfort, size);
            ViewBag.Rooms = rooms;
            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Owner")]
        public ActionResult Answer(int requestId, int roomId)
        {
            Room room = RoomAccess.GetById(roomId);
            Request request = RequestAccess.GetById(requestId);
            if (request != null && room != null)
            {
                request.Answer = roomId;
                request.Status = RequestStatus.Waiting;
                RequestAccess.Update(request);
                return RedirectToAction("List");
            }

            return RedirectToAction("Answer", new { id = requestId });
        }

        public ActionResult Response()
        {
            List<Request> requests = RequestAccess.GetRequestsByUserNameAndStatus(User.Identity.Name, RequestStatus.Waiting);
            List<Room> rooms = new List<Room>();
            foreach (var item in requests)
            {
                Room r = RoomAccess.GetById(item.Answer);
                if (r != null)
                {
                    rooms.Add(r);
                }
            }
            return View(rooms);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await RequestService.Delete(id);
            return RedirectToAction("List");
        }

        public async Task<ActionResult> Cancel(int id)
        {
            Request request = await RequestService.GetById(id);
            if (request != null)
            {
                request.Answer = 0;
                request.Status = RequestStatus.Refused;
                await RequestService.Edit(request.Id, request);
            }
            return RedirectToAction("List");
        }

        public async Task<ActionResult> More(int id)
        {
            Request request = await RequestService.GetById(id);
            Room room = new Room();
            if (request != null)
            {
                room = await RoomService.GetById(request.Answer);
            }
            return View(room);
        }

        public async Task<ActionResult> Reserve(int id)
        {
            Request request = await RequestService.GetById(id);
            if (request != null)
            {
                request.Status = RequestStatus.Executed;
                await RequestService.Edit(request.Id, request);
                return RedirectToAction("Reserve", "Room", new { id = request.Answer });
            }
            return RedirectToAction("List");
        }

        private async Task<IEnumerable<Request>> GetRequestsForAdmin()
        {
            IEnumerable<Request> resuests = await RequestService.List();
            List<Request> results = new List<Request>();
            foreach (var item in resuests)
            {
                if (item.Status == RequestStatus.New || item.Status == RequestStatus.Refused)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        private List<RequestViewModel> RequestsToModels(IEnumerable<Request> requests)
        {
            List<RequestViewModel> models = new List<RequestViewModel>();
            foreach (var item in requests)
            {
                models.Add(new RequestViewModel(item));
            }
            return models;
        }

        private async Task<IEnumerable<Request>> GetByUser(string name)
        {
            int userId = 0;
            IEnumerable<User> users = await UserService.GetList();
            foreach (var item in users)
            {
                if (item.Login == name)
                {
                    userId = item.Id;
                    break;
                }
            }
            IEnumerable<Request> resuests = await RequestService.List();
            List<Request> results = new List<Request>();
            foreach (var item in resuests)
            {
                if (item.UserId == userId)
                {
                    results.Add(item);
                }
            }
            return results;
        }
    }
}