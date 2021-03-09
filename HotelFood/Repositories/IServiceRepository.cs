
using HotelFood.Models;
using HotelFood.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Repositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Categories>> GetAvailableCategories(DateTime daydate);
        Task<OrdersSnapshotViewModel> GetOrdersSnapshot(int? companyid, DateTime? daydate);
        Task<UserCardViewModel> GetUserCardAsync(string cardToken);
        Task<IEnumerable<UserCardViewModel>> GetUserCardsAsync(QueryModel queryModel);
          Task<ServiceResponse> ProcessRequestAsync(ServiceRequest request);
        Task<bool> UploadDelivery(List<UserDayDish> data);
    }
}
