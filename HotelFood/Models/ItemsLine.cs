using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class ItemsLine
    {
        public List<int> DishesIds { get; set; }


        public MultiSelectList Dishes { get; set; }
        public int DishId { get; set; }

        public int DishCourse { get; set; }

        public int ComplexId { get; set; }

    }
}
