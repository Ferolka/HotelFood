using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Repositories
{
    public interface IComplexRepository
    {

        Task<bool> UpdateComplexDishes(Complex complex, List<string> dishes, int companyid, List<ItemsLine> dishLine);
        Task<Result> UpdateComplexDishes(Complex complex, int companyid, List<DishComplex> dishComplexes);

        Task<Complex> GetByIdAsync(int? id);
        void Remove(Complex complex);

        Task SaveChangesAsync();

        Task<Result> UpdateComplexEntity(Complex complex, List<DishComplex> dishComplexes, int companyid);
        Task<Result> ValidateComplexUpdate(Complex complex, int companyid, List<DishComplex> dishComplexes, List<DishComplex> origdishComplexes=default);
    }
}
