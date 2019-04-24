using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DAL;
using Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelController : ControllerBase
    {
        private readonly ApiHotel items;

        public HotelController(ApiHotel apiItems)
        {
            items = apiItems;
        }

        [AllowAnonymous]
        [HttpGet("List/{orderBy}/{desc}")]
        public ActionResult<List<Hotel>> List(HotelSortBy orderBy = HotelSortBy.None, bool desc = false)
        {
            return items.GetAll(orderBy, desc);
        }

        [HttpGet("{id}")]
        public ActionResult<Hotel> GetById(int id)
        {
            return items.GetById(id);
        }
        
        [HttpPost("Add")]
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult<Hotel> Add([FromBody] Hotel hotel)
        {
            return items.Create(hotel);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult<Hotel> Edit(int id, [FromBody] Hotel hotel)
        {
            return items.Update(id, hotel);
        }

        [HttpGet("Delete/{id}")]
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult<bool> Delete(int id)
        {
            return items.Delete(id);
        }
    }
}