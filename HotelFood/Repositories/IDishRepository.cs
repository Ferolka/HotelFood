using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelFood.Models;
namespace HotelFood.Repositories
{
    public interface IDishRepository
    {

        Task<bool> UpdateDishIngredients(Dish dish,int companyid);
       // Task<bool> UpdateDishIngredients(Dish dish, int companyid);
        Task<bool> UpdateDishEntity(Dish dish, int companyid);
       // IEnumerable<DishIngredientsProportionViewModel> DishIngredient(int id);
        Task<Result> ValidateWeightDish(int id, int companyid,Dish dish);
    }
}
