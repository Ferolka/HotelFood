using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using  System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelFood.Models
{
    public abstract class UserData:HotelData
    {
        [StringLength(100)]
        public string UserId { get; set; }

        public HotelUser User { get; set; }


    }
    public abstract class HotelDataOwnId: HotelData
    {
        //[StringLength(100)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
    public abstract class HotelData
    {
        
        public int HotelId { get; set; }

        public virtual Hotel Hotel { get; set; }
    }
    public static class ModelExtension
    {
        public static IQueryable<TEntity> WhereU<TEntity>(this IQueryable<TEntity> source) where TEntity : UserData
        {
            return  source.Where(u=>u.UserId =="");  //to do
        }
        public static IQueryable<TEntity> WhereCompany<TEntity>(this IQueryable<TEntity> source,int companyId) where TEntity : HotelData
        {
            return source.Where(u => u.HotelId == companyId);  //to do
        }
    }
}
