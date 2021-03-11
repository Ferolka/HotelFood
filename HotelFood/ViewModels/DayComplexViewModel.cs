using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class DayComplexViewModel
    {
        public DateTime Date { get; set; }

        public int ComplexId { get; set; }
        public string ComplexName { get; set; }


        public int CompanyId { get; set; }
        public bool Enabled { get; set; }

        public string CategoryName { get; set; }
      
        public string  DishesString { get; set; }


        public IEnumerable<DayComplexDishesViewModel> ComplexDishes { get; set; }
    }

    public class DayComplexDishesViewModel
    {
        public int DishId { get; set; }

        public string DishName { get; set; }

        public decimal DishReadyWeight { get; set; }

        public string DishDescription { get; set; }


        public string DishIngredients { get; set; }

        public int? PictureId { get; set; }
        public int DishCourse { get; set; }
    }
}
