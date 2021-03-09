using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class NotOrderedQueue : HotelDataOwnId
    {
        public int TerminalId { get; set; }
        public DateTime DayDate { get; set; }

        [StringLength(100)]
        public string UserId { get; set; }
 
        public int CategoryId { get; set; }



    }
}
