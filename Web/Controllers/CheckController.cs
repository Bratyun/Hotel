using DAL.EntityClasses;
using Hotel.DAL;
using Hotel.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Hotel.Controllers
{
    [Authorize]
    public class CheckController : Controller
    {
        private Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        
        public ActionResult List()
        {
            Logger.Debug("Request to take all list of Checks");

            List<CheckViewModel> checks = new List<CheckViewModel>();
            if (User.IsInRole("Owner"))
            {
                checks = CheckAccess.GetAllFormAdmin();
            }
            else
            {
                checks = CheckAccess.GetAllByUser(User.Identity.Name);
            }
            Logger.Debug("Response of taking all list of Checks");
            return View(checks);
        }

        [Authorize(Roles = "User")]
        public ActionResult Pay(int id)
        {
            Logger.Debug("Start of pay check");
            Check check = CheckAccess.GetById(id);
            if (check != null)
            {
                check.Status = CheckStatus.Paid;
                Logger.Debug("Success, end of pay check");
                CheckAccess.Update(check);
            }
            Logger.Error("Check not found");
            return RedirectToAction("List");
        }

        [Authorize(Roles = "User")]
        public ActionResult Delete(int id)
        {
            Logger.Debug("Start of delete check");
            Check check = CheckAccess.GetById(id);
            if (check != null)
            {
                Room room = RoomAccess.GetById(check.RoomId);
                if (room != null)
                {
                    if (DateTime.Now.Date < room.StartDate.Date || check.Status == CheckStatus.Paid || check.Status == CheckStatus.Failed)
                    {
                        RoomAccess.SetRoomFree(room.Id);
                        check.Status = CheckStatus.Stored;
                        CheckAccess.Update(check);
                        //CheckAccess.Delete(id);
                    }
                    Logger.Warn("Did not delete check because it user start use room without paying");
                }
                Logger.Error("Room in check not found");
            }
            Logger.Error("Check not found");
            return RedirectToAction("List");
        }
    }

    
}