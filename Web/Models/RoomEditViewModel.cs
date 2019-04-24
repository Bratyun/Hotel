using Contract.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RoomEditViewModel
    {
        public int Id { get; set; }
        [Range(0, 10)]
        public int RoomSize { get; set; }
        [Range(0, 5)]
        public int Comfort { get; set; }
        public int UserId { get; set; }
        public RoomStatus Status { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = false)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = false)]
        public DateTime EndDate { get; set; }
        public byte[] Image { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price need be more than 0")]
        public double Price { get; set; }
        
        public RoomEditViewModel()
        { }

        public RoomEditViewModel(Room room)
        {
            Id = room.Id;
            RoomSize = room.RoomSize;
            Comfort = room.Comfort;
            UserId = room.UserId;
            User = room.User;
            Status = room.Status;
            StartDate = room.StartDate;
            EndDate = room.EndDate;
            Image = room.Image;
            Price = room.Price;
        }
    }
}