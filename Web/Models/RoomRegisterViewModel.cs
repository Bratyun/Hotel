using Contract.Models;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RoomRegisterViewModel
    {
        [Required]
        [Range(0, 10, ErrorMessage = "Take value between 0 and 10")]
        public int RoomSize { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Take value between 0 and 5")]
        public int Comfort { get; set; }
        [Required]
        public int HotelId { get; set; }
        [Required]
        public RoomStatus Status { get; set; }
        public byte[] Image { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price need be more than 0")]
        public double Price { get; set; }
    }
}