using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.ViewModels
{
    public class OrdersSnapshotViewModel
    {
        public IEnumerable<UserDay> UserDays { get; set; }
        public IEnumerable<UserDayDish> UserDayDishes { get; set; }
        public IEnumerable<UserDayComplex> UserDayComplexes{ get; set; }

        public IEnumerable<Complex> Complexes { get; set; }

        
    }
}
