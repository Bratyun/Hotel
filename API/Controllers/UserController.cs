using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DAL;
using Contract.Consts;
using Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private ApiUser users;

        public UserController(ApiUser apiUsers)
        {
            users = apiUsers;
        }

        [HttpPost("Register")]
        public ActionResult<User> Register([FromBody] User user)
        {
            if ((user.Password.Length < 6) || (user.Phone != null && !users.IsValidPhone(user.Phone)))
            {
                return null;
            }
            return users.Create(user);
        }

        [HttpGet("List")]
        [Authorize(Roles = "Admin, Owner")]
        public ActionResult<List<User>> List()
        {
            return users.GetAll();
        }

        [HttpPost("Edit/current")]
        [Authorize]
        public ActionResult<User> Edit([FromBody] User user)
        {
            int id = int.Parse(User.FindFirst(TokenClaims.ID).Value);
            return users.Update(id, user);
        }

        [HttpPost("Edit")]
        [Authorize(Roles = "Owner")]
        public ActionResult<User> Edit(int id, [FromBody] User user)
        {
            return users.Update(id, user);
        }

        [HttpGet("Delete/{id}")]
        [Authorize(Roles = "Owner")]
        public ActionResult<bool> Delete(int id)
        {
            User user = users.GetById(id);
            if (user != null && user.Role == "Owner")
            {
                return false;
            }
            return users.Delete(id);
        }

        [HttpGet("Delete/current")]
        [Authorize]
        public ActionResult<bool> Delete()
        {
            int id = int.Parse(User.FindFirst(TokenClaims.ID).Value);
            User user = users.GetById(id);
            if (user != null && user.Role == "Owner")
            {
                return false;
            }
            return users.Delete(id);
        }
    }
}