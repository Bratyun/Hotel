using API.DAL;
using Contract.Models;
using Hotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models;

namespace Hotel.Controllers
{
    [Authorize]
    [Authorize(Roles = "Owner")]
    public class UserController : Controller
    {
        public UsersProxy UserService =>
            new UsersProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public async Task<ActionResult> List()
        {
            IEnumerable<User> users = await UserService.GetList();
            if (users == null)
            {
                return Redirect(Url.Action("Logout", "Account"));
            }
            return View(users);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            User user = await UserService.Get(id);
            if (user == null)
            {
                return Redirect(Url.Action("Logout", "Account"));
            }
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(User user)
        {
            User old = await UserService.Get(user.Id);

            if (user.Login == null)
            {
                ModelState.AddModelError("UserName", "Invalid Login");
            }
            else
            {
                User existing = await UserService.Get(user.Id);
                if (existing?.Id == user.Id)
                {
                    ModelState.AddModelError("UserName", "Sorry but user exists with same login");
                }
                else
                {
                    old.Login = user.Login;
                }
            }

            if (user.Phone != null && !Tools.IsCorrectPhoneNumber(user.Phone))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number need have form like 000-000-0000");
            }
            else
            {
                old.Phone = user.Phone;
            }
            
            if (ModelState.IsValid)
            {
                old.Role = user.Role;
                old.Email = user.Email;
                bool isOk = await UserService.Update(user, old.Id);
                return RedirectToAction("List");
            }
            return View(user);
        }

        public async Task<ActionResult> Delete(int id)
        {
            User user = await UserService.Get(id);

            if (user.Role == "Owner")
            {
                return RedirectToAction("List");
            }
            else
            {
                await UserService.Delete(user.Id);
            }

            if (User.Identity.Name == user.Login)
            {
                return RedirectToAction("Logout", "Account");
            }

            return RedirectToAction("List");
        }
    }
}