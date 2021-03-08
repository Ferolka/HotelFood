using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class HotelGuests
    {
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }

        [StringLength(100)]
        public string HotelUserId { get; set; }
        public HotelUser HotelUser { get; set; }

       
    }
}
