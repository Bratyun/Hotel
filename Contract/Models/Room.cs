using System;
using System.ComponentModel.DataAnnotations;

namespace Contract.Models
{
    public enum RoomStatus
    {
        None,
        Free,
        Booked,
        Busy,
        Closed
    }

    public enum RoomSortBy
    {
        None,
        RoomSize,
        Comfort,
        Status,
        Price
    }

    public class Room
    {
        public Room()
        {
        }

        public Room(int roomSize, int comfort, byte[] image, double price, RoomStatus status)
        {
            RoomSize = roomSize;
            Comfort = comfort;
            Price = price;
            Image = image;
            Status = status;
        }

        [Required]
        public int Id { get; set; }
        [Required]
        [Range(1, 20000)]
        public int Number { get; set; }
        [Required]
        [Range(1, 10)]
        public int RoomSize { get; set; }
        [Required]
        [Range(1, 5)]
        public int UserId { get; set; }
        public int Comfort { get; set; }
        public RoomStatus Status { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        
        public int HotelId { get; set; }
        public byte[] Image { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        

    }
}
