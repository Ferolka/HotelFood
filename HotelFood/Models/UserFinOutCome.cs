using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class UserFinOutCome : HotelData
    {
        [StringLength(100)]
        public string Id { get; set; }

        public DateTime DayDate { get; set; }
        [DefaultValue(0)]
        public int OutComeType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(0.0)]
        public decimal Amount { get; set; }
        [DefaultValue(0.0)]
        public int ItemsCount { get; set; }
        public DateTime TransactionDate { get; set; }
        [StringLength(100)]
        [DefaultValue("")]
        public string Comments { get; set; }
        //public string OrderId { get; set; }

        public UserFinance UserFinance { get; set; }
    }
}
