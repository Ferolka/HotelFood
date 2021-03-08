using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class UserFinance:HotelData
    {

        public UserFinance()
        {
           // UserFinIncomes = new HashSet<UserFinIncome>();
            UserFinOutComes = new HashSet<UserFinOutCome>();
            //   ComplexCategories = new HashSet<ComplexCategory>();
        }
        [StringLength(100)]
        public string Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(0.0)]
        public decimal  Balance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(0.0)]
        public decimal TotalIncome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOutCome { get; set; }

        [DefaultValue(0.0)]
        public int TotalOrders { get; set; }

        [DefaultValue(0)]
        public int TotalPreOrders { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(0.0)]
        public decimal TotalPreOrderedAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(0.0)]
        public decimal TotalPreOrderBalance { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(0.0)]
        public decimal DayLimit { get; set; }
        public DateTime LastUpdated { get; set; }

       // public CompanyUser CompanyUser { get; set; }
       // public virtual ICollection<UserFinIncome> UserFinIncomes { get; set; }

        public virtual ICollection<UserFinOutCome> UserFinOutComes { get; set; }
    }
}
