using Contract.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CheckViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price need be more than 0")]
        public double Price { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Result sum need be more than 0")]
        public double Total { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RegisterDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required]
        public CheckStatus Status { get; set; }

        public CheckViewModel()
        { }

        public CheckViewModel(Check check, Room room)
        {
            if (check != null && room != null)
            {
                Id = check.Id;
                UserId = check.UserId;
                RoomId = room.Id;
                Price = room.Price;
                EndDate = room.EndDate;
                StartDate = room.StartDate;
                RegisterDate = check.RegisterDate;
                Total = (EndDate - StartDate).TotalDays * Price;
                Status = check.Status;
            }
        }
    }
}