using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class UserDayDish: UserData
    {
        public DateTime Date { get; set; }

        public int DishId { get; set; }
        //public virtual ICollection<Dish> Dishes { get; set; }

        public virtual Dish Dish { get; set; }

        public int? ComplexId { get; set; }
        public virtual Complex Complex { get; set; }
        public int Quantity { get; set; }
   
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public int CategoriesId { get; set; }
        
        public bool IsComplex { get; set; }
        [DefaultValue(false)]
        public bool IsDelivered { get; set; }
    }
}
