using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class DayDishViewModel
    {
        public DateTime Date { get; set; }

        public int DishId { get; set; }
        public string DishName { get; set; }


        public int CompanyId { get; set; }
        public bool IsWeight { get; set; }
        public bool Enabled { get; set; }

        public int CategoryId { get; set; }

        public int? PictureId { get; set; }
    }

    public class DayDishViewModelPerGategory
    {
        public DayDishViewModelPerGategory()
        {
            DayDishes = new List<DayDishViewModel>();
        }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public DateTime CategoryDate { get; set; }

        public IEnumerable<DayDishViewModel> DayDishes{ get; set;}

    }
}
