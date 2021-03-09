using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelFood.Core;
using HotelFood.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelFood.Data
{
    public class AppDbContext : IdentityDbContext<HotelUser, HotelRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int hotelId = -1;
        private bool isHotelIdSet = false;
        private readonly IWebHostEnvironment _hostingEnv;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnv) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _hostingEnv = hostingEnv;
        }
        public void SetCompanyID(int val)
        {
            hotelId = val;
            isHotelIdSet = true;
        }
        public int CompanyId
        {
            get
            {
                if (isHotelIdSet)
                    return hotelId;
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
                    return _httpContextAccessor.HttpContext.User.GetHotelID();
                return hotelId;
            }
        }
        public ClaimsPrincipal CurrentUser
        {
            get
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
                    return _httpContextAccessor.HttpContext.User;
                return null;
            }
        }
        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<HotelUser> HotelUser { get; set; }
        public DbSet<HotelGuests> HotelGuests { get; set; }
        public DbSet<UserGroups> UserGroups { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Dish> Dish { get; set; }
        public DbSet<Complex> Complex { get; set; }
        public DbSet<DishComplex> DishComplex { get; set; }
        public DbSet<DayDish> DayDish { get; set; }

        public DbSet<DayComplex> DayComplex { get; set; }
        public DbSet<UserDay> UserDay { get; set; }
        public DbSet<UserDayDish> UserDayDish { get; set; }

        public DbSet<UserDayComplex> UserDayComplex { get; set; }
        public DbSet<DeliveryQueue> DeliveryQueue { get; set; }
        public DbSet<Pictures> Pictures { get; set; }
        public DbSet<UserFinance> UserFinances { get; set; }

        public DbSet<UserFinOutCome> UserFinOutComes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HotelUser>()
                 .HasOne(u => u.UserGroup)
                 .WithMany(a => a.HotelUsers)

                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HotelGuests>()
                .HasKey(cu => new { cu.HotelId, cu.HotelUserId });
            modelBuilder.Entity<HotelGuests>()
                .HasOne(cu => cu.Hotel)
                .WithMany(u => u.HotelGuests)
                .HasForeignKey(cu => cu.HotelId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<HotelGuests>()
                .HasOne(cu => cu.HotelUser)
                .WithMany(u => u.HotelGuests)
                .HasForeignKey(u => u.HotelUserId)
                .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<Dish>()
                 .HasOne(c => c.Category)
                 .WithMany(a => a.Dishes)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complex>()
                   .HasOne(c => c.Category)
                   .WithMany(a => a.Complexes)
                   .HasForeignKey(u => u.CategoriesId).IsRequired(false)
                   .OnDelete(DeleteBehavior.NoAction);
            //complex 
            modelBuilder.Entity<DishComplex>()
             .HasKey(bc => new { bc.DishId, bc.ComplexId, bc.DishCourse });
            //day dish
            modelBuilder.Entity<DishComplex>()
                 .HasOne(c => c.Dish)
                 .WithMany(a => a.DishComplex)
                 .IsRequired()

                 .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DishComplex>()
                 .HasOne(c => c.Complex)
                 .WithMany(a => a.DishComplex)
                 .IsRequired()

                 .OnDelete(DeleteBehavior.Restrict);
            

            modelBuilder.Entity<DayDish>()
                   .Property(d => d.Date)
                   .HasColumnType("date");

            modelBuilder.Entity<DayDish>()
              .HasKey(d => new { d.Date, d.DishId, d.HotelId, d.CategoriesId});
            modelBuilder.Entity<DayDish>().HasOne(d => d.Dish)
                .WithMany(d => d.DayDish)
                .HasForeignKey(d => d.DishId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<DayComplex>()
                   .Property(d => d.Date)
                   .HasColumnType("date");

            modelBuilder.Entity<DayComplex>()
              .HasKey(d => new { d.Date, d.ComplexId, d.HotelId });
            modelBuilder.Entity<DayComplex>().HasOne(d => d.Complex)
                .WithMany(d => d.DayComplex)
                .HasForeignKey(d => d.ComplexId)
                .OnDelete(DeleteBehavior.NoAction);
          

            modelBuilder.Entity<UserDayDish>()
              .HasKey(o => new { o.UserId, o.Date, o.DishId, o.HotelId, o.ComplexId, o.CategoriesId});
            //many to many Dish <-> catgories
            modelBuilder.Entity<UserDayDish>()
                   .Property(d => d.Date)
                   .HasColumnType("date");

            modelBuilder.Entity<UserDayComplex>()
            .HasKey(o => new { o.UserId, o.Date, o.ComplexId, o.HotelId });
            //many to many Dish <-> catgories
            modelBuilder.Entity<UserDayComplex>()
                   .Property(d => d.Date)
                   .HasColumnType("date");

            modelBuilder.Entity<UserDay>()
             .HasKey(o => new { o.UserId, o.Date, o.HotelId });
            modelBuilder.Entity<UserDay>()
                   .Property(d => d.Date)
                   .HasColumnType("date");
            modelBuilder.Entity<DeliveryQueue>()
                .HasIndex(p => new { p.DayDate, p.UserId, p.DishId }).IsUnique(true);



            modelBuilder.Entity<UserFinOutCome>()
             .HasKey(i => new { i.Id, i.DayDate, i.TransactionDate });
            
            modelBuilder.Entity<UserFinOutCome>()
                  .HasOne(c => c.UserFinance)
                  .WithMany(a => a.UserFinOutComes)
                  .HasForeignKey(f => new { f.Id, f.HotelId })
                  .IsRequired(true)
                  .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserFinOutCome>()
               .Property(d => d.DayDate)
               .HasColumnType("date");
            modelBuilder.Entity<UserFinOutCome>()
               .Property(d => d.TransactionDate)
               .HasDefaultValueSql("(getdate())");
            modelBuilder.Entity<UserFinOutCome>()
               .Property(d => d.TransactionDate)
               .HasColumnType("datetime")
               .HasDefaultValueSql("(getdate())");

            modelBuilder.Entity<UserFinance>()
                .HasKey(u => new { u.Id, u.HotelId });
            modelBuilder.Entity<UserFinance>()
                .Property(d => d.LastUpdated)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            modelBuilder.Entity<UserFinance>()
               .Property(d => d.Balance)
               .HasDefaultValueSql("(0.0)");

            modelBuilder.Entity<UserFinance>()
               .Property(d => d.TotalIncome)
               .HasDefaultValueSql("(0.0)");

            modelBuilder.Entity<UserFinance>()
               .Property(d => d.TotalOutCome)
               .HasDefaultValueSql("(0.0)");


            modelBuilder.Entity<UserFinance>()
               .Property(d => d.TotalOrders)
               .HasDefaultValueSql("(0)");
            modelBuilder.Entity<UserFinance>()
               .Property(d => d.TotalPreOrderedAmount)
               .HasDefaultValueSql("(0.0)");

            modelBuilder.Entity<UserFinance>()
               .Property(d => d.TotalPreOrders)
               .HasDefaultValueSql("(0)");
        }
    }
}
