using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class ComplexDishViewModel
    {
        public List<string> DishesIds { get; set; }

        
        public MultiSelectList Dishes { get; set; }
    }
}
