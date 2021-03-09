using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class UserOrderedDay
    {
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<UserDayComplexViewModel> UserDayComplex{ get; set;}
        public IEnumerable<UserDayDishViewModel> UserDayDish { get; set; }
        public bool Confirmed { get; set; }
        public decimal Total { get; set; }
        public decimal? TotalWithoutDiscount { get; set; }
        public decimal? DiscountSum { get; set; }
    }
}
