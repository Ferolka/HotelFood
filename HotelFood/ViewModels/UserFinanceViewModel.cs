using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.ViewModels
{
    public class UserFinanceViewModel
    {
        public UserFinanceViewModel()
        {
           //Incomes = new HashSet<UserFinIncome>();
            Outcomes = new HashSet<UserFinOutCome>();
        }
        public string UserId { get; set; }
        public int CompanyId { get; set; }
        public UserFinance Finance { get; set; }

       // public ICollection<UserFinIncome> Incomes { get; set; }

        public ICollection<UserFinOutCome> Outcomes { get; set; }
    }
}
