using API.DAL;
using Contract.Consts;
using Hotel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models;

namespace Hotel.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthProxy authService = new AuthProxy();
        
        public UsersProxy UserService =>
            new UsersProxy(HttpContext?.Session.GetString(SessionKeys.Token));

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string token;
                try
                {
                    token = await authService.Login(new LoginRequestModel
                    {
                        Login = model.Login,
                        Password = model.Password
                    });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Cannot connect to the API");
                    return View(model);
                }

                if (token != null && AuthenticateUser(token))
                {
                    return RedirectToAction("List", "Room");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password need consist more than 6 symbols");
            }

            if (model.Phone != null && !Tools.IsCorrectPhoneNumber(model.Phone))
            {
                ModelState.AddModelError("PhoneNumber", "Phone number need have form like 000-000-0000");
            }

            if (ModelState.IsValid)
            {
                bool isOk = false;
                try
                {
                    model.Password = model.Password;
                    isOk = await UserService.Add(UserService.Convert(model));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Cannot connect to the API");
                    return View(model);
                }

                if (isOk)
                {
                    return Redirect(Url.Action("login"));
                }
            }

            ModelState.AddModelError(string.Empty, "Cannot register this user");
            return View(model);
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect($"~/");
        }

        [HttpGet]
        public IActionResult Culture()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Culture(CultureModel model)
        {
            HttpContext.Session.SetString(SessionKeys.Culture, model.Culture);
            return Redirect(Url.Action("List", "Room"));
            //return View(model);
        }

        private bool AuthenticateUser(string token)
        {
            if (!JwtManager.ValidateToken(token, out ClaimsPrincipal principal))
            {
                return false;
            }

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.Add(AuthOptions.LIFETIME),
                IsPersistent = true
            };

            HttpContext.Session.SetString("token", token);
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(principal),
                authProperties);

            return true;
        }
    }
}