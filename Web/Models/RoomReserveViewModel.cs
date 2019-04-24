using Contract.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RoomReserveViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(0, 10, ErrorMessage = "Take value between 0 and 10")]
        public int RoomSize { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Take value between 0 and 5")]
        public int Comfort { get; set; }
        public byte[] Image { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price need be more than 0")]
        public double Price { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public RoomReserveViewModel()
        { }

        public RoomReserveViewModel(Room room)
        {
            Id = room.Id;
            RoomSize = room.RoomSize;
            Comfort = room.Comfort;
            Image = room.Image;
            Price = room.Price;
            StartDate = room.StartDate;
            EndDate = room.EndDate;
        }
    }
}