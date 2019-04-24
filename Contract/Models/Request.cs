using System;
using System.ComponentModel.DataAnnotations;

namespace Contract.Models
{
    public enum RequestStatus
    {
        None,
        New,
        Waiting,
        Executed,
        Refused
    }

    public class Request
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(1, 10)]
        public int RoomSize { get; set; }
        [Range(1, 5)]
        public int Comfort { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public RequestStatus Status { get; set; }
        public int Answer { get; set; }
    }
}
