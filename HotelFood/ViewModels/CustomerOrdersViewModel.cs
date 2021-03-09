using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class CustomerOrdersViewModel
    {
        public CustomerOrdersViewModel()
        {
            Details = new List<CustomerOrdersDetailsViewModel>();
        }
        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int DishesCount { get; set; }

        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsPaid { get; set; }
        public CompanyModel User { get; set; }
        public IEnumerable<CustomerOrdersDetailsViewModel>  Details { get; set; }
    }
    public class CustomerOrdersDetailsViewModel
    {
        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }


        public int CategoryId { get; set; }
        public int DishId { get; set; }

        public string DishName { get; set; }
        public int Quantity { get; set; }

        public Decimal Price { get; set; }
        public Decimal Amount { get; set; }
    }
}
