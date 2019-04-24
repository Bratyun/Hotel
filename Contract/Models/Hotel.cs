using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Contract.Models
{
    public enum HotelSortBy
    {
        None,
        Star
    }

    public class Hotel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [Range(1, 5)]
        public int Star { get; set; }
        public string Site { get; set; }
        public byte[] Image { get; set; }
    }
}
