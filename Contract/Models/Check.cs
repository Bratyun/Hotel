using System;
using System.ComponentModel.DataAnnotations;

namespace Contract.Models
{
    public enum CheckStatus
    {
        None,
        New,
        Paid,
        Failed
    }

    public class Check
    {
        public Check(int id1, int id2, double price, DateTime utcNow, CheckStatus @new)
        {
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoomId { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime RegisterDate { get; set; }
        [Required]
        public CheckStatus Status { get; set; }
    }
}
