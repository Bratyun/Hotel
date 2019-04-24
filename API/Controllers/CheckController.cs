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
    [Authorize]
    public class CheckController : Controller
    {
        private readonly ApiCheck items;

        public CheckController(ApiCheck apiItems)
        {
            items = apiItems;
        }

        [HttpGet("list")]
        public ActionResult<List<Check>> List()
        {
            List<Check> checks = new List<Check>();
            if (User.IsInRole("Owner"))
            {
                checks = items.GetAll();
            }
            else
            {
                checks = items.GetAllByUser(int.Parse(User.FindFirst(TokenClaims.ID).Value));
            }
            return checks;
        }

        [HttpPost("Add")]
        public ActionResult<Check> Add([FromBody] Check model)
        {
            return items.Create(model);
        }

        [HttpGet("{id}")]
        public ActionResult<Check> GetById(int id)
        {
            return items.GetById(id);
        }

        [HttpPost("Edit/{id}")]
        public ActionResult<Check> Edit(int id, [FromBody] Check model)
        {
            return items.Update(id, model);
        }
    }
}