
using HotelFood.Core;
using HotelFood.Data;
using HotelFood.Models;
using HotelFood.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HotelFood.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ServiceRepository> _logger;
        private readonly SharedViewLocalizer _localizer;
        private readonly UserManager<HotelUser> _userManager;
        private readonly IMemoryCache _cache;
        private static SemaphoreSlim register_locker = new SemaphoreSlim(1, 1);
        public ServiceRepository(AppDbContext context, ILogger<ServiceRepository> logger,
            UserManager<HotelUser> userManager, IMemoryCache cache, SharedViewLocalizer localizer)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _cache = cache;
            _localizer = localizer;

        }

        public async Task<List<DeliveryDishViewModel>> GetDishesToDeliveryAsync(string userId, DateTime dayDate, bool includeDelievered = false, int[] сategoriesIds = default)
        {
            if (сategoriesIds == null)
                сategoriesIds = new int[0];
            var  complesDishes =await (from ud in _context.UserDayDish.Where(ud => ud.UserId == userId && ud.Date == dayDate && (!ud.IsDelivered || includeDelievered))
                                 join d in _context.Dish on ud.DishId equals d.Id
                                 join c in _context.Complex on ud.ComplexId equals c.Id
                                 join dc in _context.DishComplex on new { DishId = d.Id, ComplexId = c.Id } equals new { DishId = dc.DishId, ComplexId = dc.ComplexId }
                                 where (сategoriesIds.Length == 0 || сategoriesIds.Contains(c.CategoriesId))
                                 orderby dc.DishCourse


                                 select new DeliveryDishViewModel() { ID = d.Id, Name = d.Name, IsComplex = ud.IsComplex, 
                                     DishNumber = dc.DishCourse, ComplexId = (ud.ComplexId.HasValue ? ud.ComplexId.Value : -1),
                                 CategoriesId=c.CategoriesId})
                       .ToListAsync();
            //var dishes = await (from ud in _context.UserDayDish.Where(ud => ud.UserId == userId && ud.Date == dayDate && (!ud.IsDelivered || includeDelievered)&&!ud.IsComplex)
            //                     join d in _context.Dish on ud.DishId equals d.Id
            //                     where (сategoriesIds.Length == 0 || сategoriesIds.Contains(ud.CategoriesId))
            //                     select new DeliveryDishViewModel() { ID = d.Id, Name = d.Name, IsComplex = ud.IsComplex, DishNumber = -1, ComplexId = (ud.ComplexId.HasValue ? ud.ComplexId.Value : -1),
            //                         IsWeight=ud.IsWeight,Weight= (ud.IsWeight ? ud.Quantity*(ud.Base.HasValue?ud.Base.Value:1) : ud.Quantity),MeasureUnit=d.MeasureUnit,
            //                     DishKindId=ud.DishKindId, CategoriesId=ud.CategoriesId})
            //           .ToListAsync();
           // var res = complesDishes.Union(dishes).ToList();
            return complesDishes;
        }
        public async Task<List<DeliveryQueue>> GetQueueToDeliveryAsync(string userId, DateTime dayDate)
        {
            return await _context.DeliveryQueue.Where(q => q.UserId == userId && q.DayDate == dayDate).ToListAsync();
        }
        public async Task<ServiceResponse> ProcessRequestAsync(ServiceRequest request)
        {
            var res = await PrevalidateRequestAsync(request);
            if (!res.IsSuccess())
                return res;
            switch (request.Type)
            {
                case "askfordelivery":
                    return await ProcessDeliveryRequestAsync(request);
                case "askforregister":
                    return await ProcessRegisterRequestAsync(request);
                case "askforconfirm":
                    return await ProcessConfirmRequestAsync(request);
                case "askforqueue":
                    return await ProcessQueueRequestAsync(request);
                //case "askforhistoryqueue":
                //    return await ProcessQueueHistoryRequestAsync(request);
                case "askforqueueconfirm":
                    return await ProcessQueueConfirmRequestAsync(request);
                case "askforqueueremove":
                    return await ProcessQueueRemoveRequestAsync(request);
                default:
                    break;
            }
            return ServiceResponse.GetFailResult();
        }

        public async Task<ServiceResponse> PrevalidateRequestAsync(ServiceRequest request)
        {
            var fail = ServiceResponse.GetFailResult();
            //var user = await _userManager.FindByIdAsync(request.UserId);  //now by tag
            
            HotelUser user = null;
            if (!string.IsNullOrEmpty(request.UserToken))
            {
                user = await _userManager.FindByCardTokenAsync(request.UserToken);  //now by tag
            }
            if (!string.IsNullOrEmpty(request.UserId) && user == null)
            {
                user = await _userManager.FindByIdAsync(request.UserId);  //now by tag
            }
            if (user == null && request.IsRequiredUser())
            {
                fail.ErrorMessage = "Користувач не знайдений";
                return fail;
            }
            if (request.DishesNum == null)
                request.DishesNum = new int[0];
            //TODO
            if (_context.CompanyId <= 0 && user != null)
                _context.SetCompanyID(user.HotelId);
            request.CompanyId = _context.CompanyId;
            if (user != null)
            {
                //request.CompanyId = user.CompanyId;
                request.User = user;
                request.UserId = user.Id;


            }

            if (_context.CompanyId <= 0)
                _context.SetCompanyID(request.CompanyId);

            request.DayDate = request.DayDate.ResetHMS();
            return ServiceResponse.GetSuccessResult();
        }
        public async Task<ServiceResponse> ProcessConfirmRequestAsync(ServiceRequest request)
        {
            var queue = await GetQueueToDeliveryAsync(request.UserId, request.DayDate);
            var dishes = await GetDishesToDeliveryAsync(request.UserId, request.DayDate);
            var confirmed_queue = queue.Where(q => request.DishesIds.Contains(q.DishId)).ToList();
            //var confirmed_dishes = queue.Where(d => request.DishesIds.Contains(d.DishId)).ToList();
            var user_daydishes = await _context.UserDayDish.Where(ud => ud.UserId == request.UserId && ud.Date == request.DayDate && !ud.IsDelivered).ToListAsync();
            var user_daydishes_confirmed = user_daydishes.Where(q => request.DishesIds.Contains(q.DishId)).ToList();
            try
            {
                user_daydishes_confirmed.ForEach(d => d.IsDelivered = true);
                _context.UpdateRange(user_daydishes_confirmed);
                await _context.SaveChangesAsync();
                _context.RemoveRange(confirmed_queue);
                // await _context.SaveChangesAsync();
                await SaveChangesLoopAsync();
                return ServiceResponse.GetSuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error save confirmation");
            }
            return ServiceResponse.GetFailResult();
        }
        private async Task SaveChangesLoopAsync()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    break;
                }
                catch (DbUpdateException dbex)
                {
                    // get failed entries
                    try
                    {
                        var entries = dbex.Entries;
                        foreach (var entry in entries)
                        {
                            // change state to remove it from context 
                            entry.State = EntityState.Detached;
                        }
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error save confirmation in the loop");
                    break;
                }
            }
        }
        public async Task<ServiceResponse> ProcessQueueRequestAsync(ServiceRequest request)
        {
            /* not working on EF core
            var query=from q in _context.DeliveryQueues.Where(q=>q.DayDate==request.DayDate)
                      from ud in _context.UserDayDish.Where(ud =>  ud.Date == request.DayDate)
                      from d in _context.Dishes
                      from u in _context.CompanyUser
                      where (ud.DishId==q.DishId && d.Id==ud.DishId && u.Id==q.UserId)
                      group new {userid=u.Id, dishid = d.Id, dishname = d.Name }  by u.Id into grp
                      select new ServiceResponse()
                      { UserId=grp.Key,
                          Dishes =from it in grp 
                                   select new DeliveryDishViewModel() { 
                                   ID=it.dishid,
                                   Name=it.dishname
                                   }
                                  
                      };
            */
            try
            {
                var query = await (from q in _context.DeliveryQueue.Where(q => q.DayDate == request.DayDate.ResetHMS()
                                   && q.Id > request.LastQueueId)
                                   join ud in _context.UserDayDish on new { q.DishId, DayDate = q.DayDate, q.UserId, ComplexId=q.ComplexId } equals new { ud.DishId, DayDate = ud.Date, ud.UserId, ComplexId=ud.ComplexId.Value }
                                   join d in _context.Dish on q.DishId equals d.Id
                                   join u in _context.HotelUser on q.UserId equals u.Id
                                   join c in _context.Complex on ud.ComplexId equals c.Id into proto
                                   from comDef in proto
                                   where request.DishesNum.Contains(q.DishCourse)
                                         && (request.ComplexCategoriesIds.Length == 0 || comDef.Id==0 ||   request.ComplexCategoriesIds.Contains(comDef.CategoriesId))
                                   // select new { userid = u.Id, dishid = d.Id, dishname = d.Name }
                                  // orderby q.Id
                                   select new DeliveryQueueForGroup()
                                   {
                                       QueueId = q.Id,
                                       UserId = u.Id,
                                       UserName = u.NameSurname,
                                       IsComplex = true,
                                       //Weight = ud.IsWeight? (ud.Quantity*(decimal)ud.Base):1,
                                       DishId = d.Id,
                                       DishName = d.Name,
                                       //UserPictureId = u.PictureId,
                                       DishCource = q.DishCourse
                                   }
                                   )
                                   //.Take((request.MaxQueue +1)* request.DishesNum.Count()+1)
                                    //.Any(value => request.DishesNum.Contains(value.DishCource)))
                                    .ToListAsync();
                if (request.NotComplexDishes)
                {
                    var query1 = await (from q in _context.DeliveryQueue.Where(q => q.DayDate == request.DayDate.ResetHMS()
                                  && q.Id > request.LastQueueId)
                                       join ud in _context.UserDayDish on new { q.DishId, DayDate = q.DayDate, q.UserId, ComplexId = q.ComplexId } equals new { ud.DishId, DayDate = ud.Date, ud.UserId, ComplexId = ud.ComplexId.Value }
                                       join d in _context.Dish on q.DishId equals d.Id
                                       join u in _context.HotelUser on q.UserId equals u.Id
                                       //join c in _context.Complex on ud.ComplexId equals c.Id into proto
                                       //from comDef in proto
                                       where ud.IsComplex==false
                                            // && (request.ComplexCategoriesIds.Length == 0 || comDef.Id == 0 || request.ComplexCategoriesIds.Contains(comDef.CategoriesId))
                                       // select new { userid = u.Id, dishid = d.Id, dishname = d.Name }
                                       // orderby q.Id
                                       select new DeliveryQueueForGroup()
                                       {
                                           QueueId = q.Id,
                                           UserId = u.Id,
                                           UserName = u.NameSurname,
                                           IsComplex = false,
                                           //IsWeight = ud.IsWeight,
                                          // Weight = ud.IsWeight ? (ud.Quantity * (decimal)ud.Base) : ud.Quantity,
                                          // MeasureUnit = ud.IsWeight ? d.MeasureUnit:_localizer["pieces"],
                                           DishId = d.Id,
                                           DishName = d.Name,
                                          // UserPictureId = u.PictureId,
                                           DishCource = q.DishCourse
                                       }
                                  ).Take((request.MaxQueue + 1) * request.DishesNum.Count() + 1)
                                   //.Any(value => request.DishesNum.Contains(value.DishCource)))
                                   .ToListAsync();
                    query = query.Union(query1).ToList();
                }
                var client_query = from q in query
                                   group q by new { q.UserId, q.UserName, q.UserPictureId } into grp
                                   select new ServiceResponse()
                                   {
                                       UserId = grp.Key.UserId,
                                       UserName = grp.Key.UserName,
                                       UserPictureId = grp.Key.UserPictureId,
                                       Dishes = from it in grp
                                                select new DeliveryDishViewModel()
                                                {
                                                    ID = it.DishId,
                                                    QueueId = it.QueueId,
                                                    IsComplex=it.IsComplex,
                                                    IsWeight = it.IsWeight,
                                                    MeasureUnit=it.MeasureUnit,
                                                    Weight = it.Weight,
                                                    Name = it.DishName
                                                }

                                   };

                var resp = ServiceQueueResponse.GetSuccessResult();
                resp.Queues = client_query.Take(request.MaxQueue).ToList();
                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessQueueRequestAsync");
                return ServiceQueueResponse.GetFailResult();
            }
        }

        //public async Task<ServiceResponse> ProcessQueueHistoryRequestAsync(ServiceRequest request)
        //{

        //    try
        //    {
        //        var query = await (from q in _context.DeliveryQueues_history.Where(q => q.DayDate == request.DayDate.ResetHMS()
        //                          // && q.Id > request.LastQueueId
        //                          )
        //                           join ud in _context.UserDayDish on new { q.DishId, DayDate = q.DayDate, q.UserId, ComplexId = q.ComplexId } equals new { ud.DishId, DayDate = ud.Date, ud.UserId, ComplexId = ud.ComplexId.Value }
        //                           join d in _context.Dishes on q.DishId equals d.Id
        //                           join u in _context.CompanyUser on q.UserId equals u.Id
        //                           join c in _context.Complex on ud.ComplexId equals c.Id
        //                           where request.DishesNum.Contains(q.DishCourse)
        //                                 && (request.ComplexCategoriesIds.Length == 0 || request.ComplexCategoriesIds.Contains(c.CategoriesId))
        //                            orderby q.Id descending
        //                           select new DeliveryQueueForGroup()
        //                           {
        //                               QueueId = q.Id,
        //                               UserId = u.Id,
        //                               UserName = u.GetChildUserName(),
        //                               DishId = d.Id,
        //                               DishName = d.Name,
        //                               UserPictureId = u.PictureId,
        //                               DishCource = q.DishCourse
        //                           }
        //                           ).Take((request.MaxQueue + 1) * request.DishesNum.Count() + 1)
        //                            //.Any(value => request.DishesNum.Contains(value.DishCource)))
        //                            .ToListAsync();
        //        if (request.NotComplexDishes)
        //        {
        //            var query1 = await (from q in _context.DeliveryQueues.Where(q => q.DayDate == request.DayDate.ResetHMS()
        //                          && q.Id > request.LastQueueId)
        //                                join ud in _context.UserDayDish on new { q.DishId, DayDate = q.DayDate, q.UserId, ComplexId = q.ComplexId } equals new { ud.DishId, DayDate = ud.Date, ud.UserId, ComplexId = ud.ComplexId.Value }
        //                                join d in _context.Dishes on q.DishId equals d.Id
        //                                join u in _context.CompanyUser on q.UserId equals u.Id
        //                                //join c in _context.Complex on ud.ComplexId equals c.Id into proto
        //                                //from comDef in proto
        //                                where ud.IsComplex == false
        //                                // && (request.ComplexCategoriesIds.Length == 0 || comDef.Id == 0 || request.ComplexCategoriesIds.Contains(comDef.CategoriesId))
        //                                // select new { userid = u.Id, dishid = d.Id, dishname = d.Name }
        //                                // orderby q.Id
        //                                select new DeliveryQueueForGroup()
        //                                {
        //                                    QueueId = q.Id,
        //                                    UserId = u.Id,
        //                                    UserName = u.GetChildUserName(),
        //                                    IsComplex = false,
        //                                    IsWeight = ud.IsWeight,
        //                                    Weight = ud.IsWeight ? (ud.Quantity * (decimal)ud.Base) : ud.Quantity,
        //                                    MeasureUnit = ud.IsWeight ? d.MeasureUnit : _localizer["pieces"],
        //                                    DishId = d.Id,
        //                                    DishName = d.Name,
        //                                    UserPictureId = u.PictureId,
        //                                    DishCource = q.DishCourse
        //                                }
        //                          ).Take((request.MaxQueue + 1) * request.DishesNum.Count() + 1)
        //                           //.Any(value => request.DishesNum.Contains(value.DishCource)))
        //                           .ToListAsync();
        //            query = query.Union(query1).ToList();
        //        }
        //        var client_query = from q in query
        //                           group q by new { q.UserId, q.UserName, q.UserPictureId } into grp
        //                           select new ServiceResponse()
        //                           {
        //                               UserId = grp.Key.UserId,
        //                               UserName = grp.Key.UserName,
        //                               UserPictureId = grp.Key.UserPictureId,
        //                               Dishes = from it in grp
        //                                        select new DeliveryDishViewModel()
        //                                        {
        //                                            ID = it.DishId,
        //                                            QueueId = it.QueueId,
        //                                            Name = it.DishName
        //                                        }

        //                           };

        //        var resp = ServiceQueueResponse.GetSuccessResult();
        //        resp.Queues = client_query.Take(request.MaxQueue).ToList();
        //        return resp;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "ProcessQueueHistoryRequestAsync");
        //        return ServiceQueueResponse.GetFailResult();
        //    }
        //}
        
        public async Task<ServiceResponse> ProcessQueueConfirmRequestAsync(ServiceRequest request)
        {
            // var res=ServiceResponse.GetSuccessResult(request);
            var queue = await _context.DeliveryQueue.Where(q => request.QueueIds.Contains(q.Id)).ToListAsync();
            //var confirmed_dishesid = queue.Select(q => q.DishId);
            var confirmed_dishes = queue.Select(q=>new { DishId = q.DishId ,ComplexId=q.ComplexId,IsComplex=q.IsComplex
                ,CategoriesId=q.CategoriesId});
            //var confirmed_complexes = queue.Select(q => q.DishId);
            //var confirmed_dishes = queue.Where(d => request.DishesIds.Contains(d.DishId)).ToList();
            var user_daydishes = await _context.UserDayDish.Where(ud => ud.UserId == request.UserId && ud.Date == request.DayDate.ResetHMS() && ud.IsDelivered == false).ToListAsync();
            //var user_daydishes_confirmed = user_daydishes.Where(q => confirmed_dishesid.Contains(q.DishId)).ToList();
            var user_daydishes_confirmed = user_daydishes.Where(q => confirmed_dishes.Any(conf=>(conf.DishId==q.DishId && conf.ComplexId==q.ComplexId&&conf.IsComplex==false&&conf.CategoriesId==q.CategoriesId)||
            (conf.DishId == q.DishId && conf.ComplexId == q.ComplexId && conf.IsComplex == true))).ToList();
            try
            {
                user_daydishes_confirmed.ForEach(d => d.IsDelivered = true);
                _context.UpdateRange(user_daydishes_confirmed);
                _context.RemoveRange(queue);
                await _context.SaveChangesAsync();
                return ServiceResponse.GetSuccessResult(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error save confirmation");
                return ServiceResponse.GetFailResult(request);
            }

            //return res;
        }
        public async Task<ServiceResponse> ProcessQueueRemoveRequestAsync(ServiceRequest request)
        {
            // var res=ServiceResponse.GetSuccessResult(request);
            var queue = await _context.DeliveryQueue.Where(q => request.QueueIds.Contains(q.Id)).ToListAsync();

            try
            {
                try
                {
                    _logger.LogWarning($"Remove queue at {DateTime.Now.ToString()} ");
                    queue.ForEach(q => _logger.LogWarning($"Queue id={q.Id} DishId={q.DishId} removed by user"));
                }
                catch { }
                _context.RemoveRange(queue);
                await _context.SaveChangesAsync();
                return ServiceResponse.GetSuccessResult(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error remove queue", ex);
                return ServiceResponse.GetFailResult(request);
            }

            //return res;
        }
        private async Task InternalRegisterQueueAsync(ServiceRequest request, List<DeliveryDishViewModel> src)
        {
            _logger.LogWarning("Enter InternalRegisterQueueAsync");
            await register_locker.WaitAsync();
            try
            {
                _logger.LogWarning("Enter critical InternalRegisterQueueAsync");
                Thread.Sleep(1000);
                
                var queue = await _context.DeliveryQueue.Where(dq => dq.UserId == request.UserId && dq.DayDate == request.DayDate.ResetHMS()).ToListAsync();
                var queue_to_add = src.Where(d => !queue.Any(q => q.DishId == d.ID && q.ComplexId == d.ComplexId)).
                    Select((q, idx) => new DeliveryQueue() { UserId = request.UserId, DishId = q.ID, ComplexId = q.ComplexId, DayDate = request.DayDate.ResetHMS(), HotelId = request.CompanyId, DishCourse = q.DishNumber, TerminalId = request.TerminalId });
                //var queue= await _context.DeliveryQueues.FirstOrDefaultAsync(ud => ud.UserId == request.UserId && ud.DayDate == request.DayDate);

                if (queue_to_add.Count() > 0)
                {
                    await _context.AddRangeAsync(queue_to_add);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding delivery Queue");


            }
            finally
            {
                register_locker.Release();
                _logger.LogWarning("Exit InternalRegisterQueueAsync");
            }
        }
         public async Task<ServiceResponse> ProcessRegisterRequestAsync(ServiceRequest request)
        {
            var fail = ServiceResponse.GetFailResult(request);


            var dishes = await GetDishesToDeliveryAsync(request.UserId, request.DayDate, false, request.ComplexCategoriesIds);

            if (dishes.Count() == 0)
            {
                fail.ErrorMessage = " Немає страв до видачі";
                await SaveNotOrderedQueueAsync(request);
                return fail;
            }
            var resp = ServiceResponse.GetSuccessResult(request);

            resp.Dishes = dishes;
            // to do
            //_ = InternalRegisterQueueAsync(request, dishes);
           // return resp;
           await register_locker.WaitAsync();

            try
            {
             
            var queue = await _context.DeliveryQueue.Where(dq => dq.UserId == request.UserId && dq.DayDate == request.DayDate.ResetHMS()).ToListAsync();
            var queue_to_add = dishes.Where(d => !queue.Any(q => q.DishId == d.ID && q.ComplexId==d.ComplexId)).
                Select((q, idx) => new DeliveryQueue() { UserId = request.UserId, DishId = q.ID,ComplexId=q.ComplexId,
                    DayDate = request.DayDate.ResetHMS(), HotelId = request.CompanyId, DishCourse = q.DishNumber,
                    TerminalId = request.TerminalId, IsComplex=q.IsComplex,CategoriesId=q.CategoriesId });
            //var queue= await _context.DeliveryQueues.FirstOrDefaultAsync(ud => ud.UserId == request.UserId && ud.DayDate == request.DayDate);
           
                if (queue_to_add.Count() > 0)
                {
                    await _context.AddRangeAsync(queue_to_add);
                    await _context.SaveChangesAsync();
                }

                return resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding delivery Queue");
                fail.ErrorMessage = "Internal Error";
                return fail;
            }
            finally
            {
                register_locker.Release();
            }
        }

        public async Task<ServiceResponse> ProcessDeliveryRequestAsync(ServiceRequest request)
        {

            var uday = await _context.UserDay.FirstOrDefaultAsync(ud => ud.UserId == request.UserId && ud.Date == request.DayDate);
            if (uday == null || !uday.IsConfirmed)
            {
                var fail = ServiceResponse.GetFailResult();
                fail.ErrorMessage = "No confirmed orders on this day";
                return fail;
            }
            // to do add dishNotComplex
            var dishes = await GetDishesToDeliveryAsync(request.UserId, request.DayDate, false);
            //if (request.NotComplexDishes)
            //{
            //    var udayDish = await (from uc in _context.UserDayDish.Where(ud => ud.UserId == request.UserId && !ud.IsComplex && ud.Date == request.DayDate.ResetHMS())


            //                          select new { ComplexId = 0, DishId = uc.DishId, ComplexName = "", Number = -1, isComplex = true }
            //                             )
            //                            .ToListAsync();
            //    dishes = dishes.Union(udayDish).ToList();
            //}
            if (dishes.Any(d => d.IsComplex))
            {
                var udaycomplex = await (from uc in _context.UserDayComplex.Where(ud => ud.UserId == request.UserId && ud.Date == request.DayDate.ResetHMS())
                                         join c in _context.Complex on uc.ComplexId equals c.Id
                                         join dc in _context.DishComplex on uc.ComplexId equals dc.ComplexId

                                         select new { ComplexId = c.Id, DishId = dc.DishId, ComplexName = c.Name, Number = dc.DishCourse,isComplex=false }
                                         )
                                        .ToListAsync();
                var dishesid_todelivery = udaycomplex.Where(it => request.DishesNum.Contains(it.Number + 1));
                dishes = dishes.Where(d => dishesid_todelivery.Any(v => v.DishId == d.ID)).ToList();
            }
            
            var response = ServiceResponse.GetSuccessResult();
            response.Dishes = dishes;
            return response;
        }
        public async Task<IEnumerable<UserCardViewModel>> GetUserCardsAsync(QueryModel queryModel)
        {
            return await _context.HotelUser
                 .Where(u =>
                         string.IsNullOrEmpty(queryModel.SearchCriteria)
                         ||
                         u.UserName.Contains(queryModel.SearchCriteria)
                         ||
                        // u.ChildNameSurname.Contains(queryModel.SearchCriteria)
                     //    ||
                         u.Email.Contains(queryModel.SearchCriteria)
                         ||
                         u.NameSurname.Contains(queryModel.SearchCriteria)
                         )
                 .Take(50)
                 .Select(u => new UserCardViewModel()
                 {
                     UserId = u.Id,
                     UserName = u.NameSurname,
                    // UserChildName = u.ChildNameSurname,
                     UserLogin = u.UserName,
                     UserEmail = u.Email,
                     CardToken = u.CardTag,
                   //  PictureId = u.PictureId

                 }
                ).ToListAsync();
        }
        public async Task<UserCardViewModel> GetUserCardAsync(string cardToken)
        {
            if (string.IsNullOrEmpty(cardToken))
                return null;
            var user = await _userManager.FindByCardTokenAsync(cardToken);
            if (user == null)
                return null;
            var res = new UserCardViewModel()
            {
                UserId = user.Id,
                UserName = user.NameSurname,
               // UserChildName = user.ChildNameSurname,
                UserLogin = user.UserName,
                UserEmail = user.Email,
                CardToken = user.CardTag,
               // PictureId = user.PictureId

            };
            return res;

        }

        public async Task<IEnumerable<Categories>> GetAvailableCategories(DateTime daydate)
        {
            //var query = await (from ud in _context.UserDayDish.Where(ud => ud.Date == daydate.ResetHMS())
            //                   join c in _context.Complex on ud.ComplexId equals c.Id
            //                   join cat in _context.Categories on c.CategoriesId equals cat.Id
            //                   select cat).Distinct().ToListAsync();
            //var query1 = await (from ud in _context.UserDayDish.Where(ud => ud.Date == daydate.ResetHMS() && !ud.IsComplex)
            //                    join cat in _context.Categories on ud.CategoriesId equals cat.Id
            //                    select cat).Distinct().ToListAsync();
            //var res = query.Union(query1).ToList();
            var res = await (from ud in _context.UserDayDish.Where(ud => ud.Date == daydate.ResetHMS())
                             join c in _context.Complex on ud.ComplexId equals c.Id
                             join cat in _context.Categories on c.CategoriesId equals cat.Id
                             select cat).Union(from ud in _context.UserDayDish.Where(ud => ud.Date == daydate.ResetHMS() && !ud.IsComplex)
                                               join cat in _context.Categories on ud.CategoriesId equals cat.Id
                                               select cat).Distinct().OrderBy(cat => cat.Code).ToListAsync();
            return res;
        }

        public async Task<OrdersSnapshotViewModel> GetOrdersSnapshot(int? companyid, DateTime? daydate)
        {
            DateTime day = daydate.HasValue ? daydate.Value.ResetHMS() : DateTime.Now.ResetHMS();
            int cid = companyid.HasValue ? companyid.Value : 1;
            var res = new OrdersSnapshotViewModel();
            res.UserDays = await _context.UserDay.IgnoreQueryFilters().Where(ud => ud.Date == day && ud.HotelId == cid).ToListAsync();
            res.UserDayComplexes = await _context.UserDayComplex.IgnoreQueryFilters().Where(ud => ud.HotelId == cid && ud.Date == day).ToListAsync();
            var complexlistIds = res.UserDayComplexes.Select(uc => uc.ComplexId).ToList();
            res.Complexes = await _context.Complex.IgnoreQueryFilters().Where(c => complexlistIds.Contains(c.Id))
                .Include(c => c.DishComplex)
               .ToListAsync();
            res.UserDayDishes = await _context.UserDayDish.IgnoreQueryFilters().Where(ud => ud.Date == day && ud.HotelId == cid).ToListAsync();

            return res;
        }
        private async Task<bool> SaveNotOrderedQueueAsync(ServiceRequest request)
        {
            try
            {
                if (request.ComplexCategoriesIds.Length == 0)
                {
                    var notorder = new NotOrderedQueue() { HotelId = _context.CompanyId, UserId = request.UserId, DayDate = request.DayDate, CategoryId = 0 };
                    _context.Add(notorder);
                }
                else
                {
                    request.ComplexCategoriesIds.ToList().ForEach(cat =>
                    {
                        var notorder = new NotOrderedQueue() { HotelId = _context.CompanyId, UserId = request.UserId, DayDate = request.DayDate, CategoryId = cat };
                        _context.Add(notorder);
                    });
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error SaveNotOrderedQueueAsync");
            }
            return true;
        }

        public async Task<bool> UploadDelivery(List<UserDayDish> data)
        {
            _logger.LogInformation("Start upload delivery");
            try
            {
               var data_bycompany = data.Where(d=>d.IsDelivered).GroupBy(d => d.HotelId); 
               foreach(var companygroup in data_bycompany)
               {
                   
                    int companyId = companygroup.Key;
                    _logger.LogInformation($"Processing company with id={companyId}");
                    _context.SetCompanyID(companyId);
                    foreach (var entry in companygroup)
                    {
                        var src = _context.UserDayDish.Where(ud => ud.Date == entry.Date && ud.UserId == entry.UserId && ud.DishId == entry.DishId && ud.ComplexId == entry.ComplexId).FirstOrDefault();
                        if (src == null)
                        {
                            _logger.LogWarning($"UserDayDish entry UserId={entry.UserId} Date={entry.Date} DishId={entry.DishId} ComplexId={entry.ComplexId} is not found in source database");
                            continue;
                        }
                        if (src.IsDelivered)
                            continue;
                        src.IsDelivered = true;
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, "failed to update entry");
                            //exclude for the further update
                            _context.Entry(src).State = EntityState.Detached;
                        }
                    }
               }
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UploadDelivery");
                return false;
            }
            return true;
        }
    }
}
