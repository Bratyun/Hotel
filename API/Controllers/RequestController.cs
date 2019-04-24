using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DAL;
using Contract.Consts;
using Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly ApiRequest items;

        public RequestController(ApiRequest apiItems)
        {
            items = apiItems;
        }

        [HttpGet("list")]
        public ActionResult<List<Request>> List()
        {
            List<Request> requests = new List<Request>();
            if (User.IsInRole("Admin"))
            {
                requests = items.GetAll().Where(x => x.Status == RequestStatus.New || x.Status == RequestStatus.Refused).ToList();
            }
            else
            {
                requests = items.GetAll().Where(x => x.UserId == int.Parse(User.FindFirst(TokenClaims.ID).Value)).ToList();
            }
            return requests;
        }

        [HttpPost("add")]
        public ActionResult<Request> Add([FromBody] Request model)
        {
            return items.Create(model);
        }

        [HttpGet("{id}")]
        public ActionResult<Request> GetById(int id)
        {
            return items.GetById(id);
        }

        [HttpGet("byStatus/{status}")]
        public ActionResult<List<Request>> GetByStatus(RequestStatus status)
        {
            return items.GetAll().Where(x => x.Status == status).ToList();
        }

        [HttpPost("Edit/{id}")]
        public ActionResult<Request> Edit(int id, [FromBody] Request model)
        {
            return items.Update(id, model);
        }


    }
}