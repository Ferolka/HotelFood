using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace HotelFood.Models
{
    public class DayComplex:HotelData
    {
        public DateTime Date { get; set; }

        public int ComplexId { get; set; }
        //public virtual ICollection<Dish> Dishes { get; set; }

        public virtual Complex Complex { get; set; }
    }
}
