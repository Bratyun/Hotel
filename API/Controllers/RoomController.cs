using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DAL;
using Contract.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomController : Controller
    {
        private readonly ApiRoom items;

        public RoomController(ApiRoom apiItems)
        {
            items = apiItems;
        }

        [AllowAnonymous]
        [HttpGet("List/{orderBy}/{desc}")]
        public ActionResult<List<Room>> List(RoomSortBy orderBy = RoomSortBy.None, bool desc = false)
        {
            return items.GetAll(orderBy, desc);
        }

        [HttpGet("{id}")]
        public ActionResult<Room> GetById(int id)
        {
            return items.GetById(id);
        }

        [HttpGet("byHotel/{id}/{orderBy}/{desc}")]
        [AllowAnonymous]
        public ActionResult<List<Room>> GetByHotel(int id, RoomSortBy orderBy = RoomSortBy.None, bool desc = false)
        {
            return items.GetAll(orderBy, desc).Where(x => x.HotelId == id).ToList();
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult<Room> Add([FromBody] Room room)
        {
            return items.Create(room);
        }

        [HttpPost("Edit/{id}")]
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult<Room> Edit(int id, [FromBody] Room room)
        {
            return items.Update(id, room);
        }

        [HttpGet("Delete/{id}")]
        [Authorize(Roles = "Owner, Admin")]
        public ActionResult<bool> Delete(int id)
        {
            return items.Delete(id);
        }
    }
}