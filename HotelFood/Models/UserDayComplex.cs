using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class UserDayComplex: UserData
    {
        public DateTime Date { get; set; }

        public int ComplexId { get; set; }

        //public DishComplex Dishes { get; set; }
        //public virtual ICollection<Dish> Dishes { get; set; }

        public virtual Complex Complex { get; set; }


        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
