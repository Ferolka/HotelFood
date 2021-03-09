using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class WeekReportModel
    {
        public string Number { get; set; }
        public CompanyModel Seller { get; set; }
        public CompanyModel Buyer { get; set; }
        public IEnumerable<DayReportModel> Items { get; set; }

        
    }


    public class DayReportModel
    {
       public DateTime Date { get; set; }
        
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "0.00")]
        public decimal Total { get; set; }
        [DisplayFormat(DataFormatString ="0.00") ]
        public decimal TotalWithoutDiscount { get; set; }
        [DisplayFormat(DataFormatString = "0.00")]
        public decimal Discount { get; set; }
        public IEnumerable<UserOrderedDay> DishesPerCategory{ get; set; }
        public bool Enabled { get; set; }
    }
    public class CompanyModel
    {
        public string Name { get; set; }
        public string ChildName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }

        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public int? PictureId { get; set; }

        public int? StampPictureId { get; set; }

        public string UrlPicture { get; set; }
        public string UrlStampPicture { get; set; }
    }
}