using DAL.Account;
using DAL.EntityClasses;
using DAL.Models;
using Hotel.DAL;
using Hotel.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        public ActionResult List()
        {
            Logger.Debug("Request to take all list of Requests");
            List<Request> requests = new List<Request>();
            if (User.IsInRole("Admin"))
            {
                requests = RequestAccess.GetRequestsForAdmin();
            }
            else
            {
                requests = RequestAccess.GetRequestsByUserName(User.Identity.Name, UserManager);
            }
            Logger.Debug("Response of taking all list of Requests");
            return View(RequestAccess.RequestsToModels(requests));
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
            Logger.Debug("Request to take all list of Requests and Rooms for Answer");
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
            Logger.Debug("Response of taking all list of Requests and Rooms for Answer");
            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Owner")]
        public ActionResult Answer(int requestId, int roomId)
        {
            Logger.Debug("Make supply");
            Room room = RoomAccess.GetById(roomId);
            Request request = RequestAccess.GetById(requestId);
            if (request != null && room != null)
            {
                request.Answer = roomId;
                request.Status = RequestStatus.Waiting;
                RequestAccess.Update(request);
                return RedirectToAction("List");
            }
            else
            {
                Logger.Error("Room or Request not found");
            }

            Logger.Debug("End of making supply");
            return RedirectToAction("Answer", new { id = requestId });
        }

        public ActionResult Response()
        {
            Logger.Debug("Start making supply for user");
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
            Logger.Debug("Show supply for user");
            return View(rooms);
        }

        public ActionResult Delete(int id)
        {
            Logger.Debug("Start of delete of Request");
            RequestAccess.Delete(id);
            Logger.Debug("End of delete of Request");
            return RedirectToAction("List");
        }

        public ActionResult Cancel(int id)
        {
            Logger.Debug("Start of refuse from Request");
            Request request = RequestAccess.GetById(id);
            if (request != null)
            {
                request.Answer = 0;
                request.Status = RequestStatus.Refused;
                RequestAccess.Update(request);
            }
            Logger.Debug("End of refuse from Request");
            return RedirectToAction("List");
        }

        public ActionResult More(int id)
        {
            Logger.Debug("Get more information about recommend room");
            Request request = RequestAccess.GetById(id);
            Room room = new Room();
            if (request != null)
            {
                room = RoomAccess.GetById(request.Answer);
            }
            else
            {
                Logger.Error("Request from user not found");
            }
            Logger.Debug("Get more information about recommend room. End");
            return View(room);
        }

        public ActionResult Reserve(int id)
        {
            Logger.Debug("Start rdirect to reserve room");
            Request request = RequestAccess.GetById(id);
            if (request != null)
            {
                request.Status = RequestStatus.Executed;
                RequestAccess.Update(request);
                return RedirectToAction("Reserve", "Room", new { id = request.Answer });
            }
            else
            {
                Logger.Error("Request from user not found");
            }
            Logger.Debug("End rdirect to reserve room");
            return RedirectToAction("List");
        }
    }
}