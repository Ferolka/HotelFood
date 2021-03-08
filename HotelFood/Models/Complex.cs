using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelFood.Models
{
    public class Complex: HotelDataOwnId
    {
        public Complex()
        {
            DishComplex = new HashSet<DishComplex>();
         //   ComplexCategories = new HashSet<ComplexCategory>();
        }

        // public int Id { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [DataType(DataType.Text)]
        [Required]
        [DisplayName("Complex Name")]
        public string Name { get; set; }

        [DisplayName("Complex Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [DisplayName("DishesQuantity")]
        public int DishesQuantity { get; set; }

        [DisplayName("ComplexCategory")]
      
        public int CategoriesId { get; set; }

        [DisplayName("Complex Category")]
        public virtual Categories Category { get; set; }
        [DisplayName("ComplexKind")]
        public int? DishKindId { get; set; }

        

        [DisplayName("Complex Dishes")]
        public virtual ICollection<DishComplex> DishComplex { get; set; }

        //public virtual ICollection<ComplexCategory> ComplexCategories { get; set; }

    }
}
