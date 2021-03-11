using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Models;
namespace HotelFood.Repositories
{
    public interface IDayDishesRepository
    {

        IQueryable<DayDishViewModel> DishesPerDay(DateTime dayDate, int companyid);
        IQueryable<DayDishViewModelPerGategory> CategorizedDishesPerDay(DateTime dayDate, int companyid);
        IQueryable<DayDishViewModelPerGategory> EnabledDishesPerDay(DateTime daydate,int companyid);

        DayDish SelectSingleOrDefault(int dishId, DateTime daydate);
        DayDish SelectSingleOrDefault(DayDish src);

        DayComplex SelectComplexSingleOrDefault(int complexId, DateTime daydate);

        DayComplex SelectComplexSingleOrDefault(DayComplex src);
        IQueryable<DayComplexViewModel> ComplexDay(DateTime daydate, int companyid);
        Task<bool> CopyDayMenu(DateTime daydate_from, DateTime daydate_to, int days, int companyId);
    }
}
