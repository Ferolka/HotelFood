using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using HotelFood.Core;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;
using System.Xml.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Drawing2D;
using HotelFood.ViewModels;

namespace HotelFood.Repositories
{
    public class UserDayDishesRepository : IUserDayDishesRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HotelUser> _logger;
        private readonly UserManager<HotelUser> _userManager;
        private readonly IMemoryCache _cache;
        //private readonly IPluginsRepository _plugins;
        public UserDayDishesRepository(AppDbContext context, ILogger<HotelUser> logger,
            UserManager<HotelUser> userManager, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _cache = cache;
           // _plugins = plugins;
        }
        public OrderTypeEnum GetCompanyOrderType(int companyid)
        {
            return _cache.GetCachedCompanyAsync(_context, companyid).Result.GetOrderType();
        }
        public bool GetConfrimedAdmin(string userid)
        {
            return _context.Users.Where(us => us.Id == userid).Select(us => us.ConfirmedByAdmin).FirstOrDefault();
        }
        public bool IsBalancePositive(string userid)
        {
            return _context.UserFinances.Where(us => us.Id == userid).Select(us => us.TotalPreOrderBalance).FirstOrDefault() > 0;
        }
        public bool IsAllowDayEdit(DateTime dt, int companyid)
        {
            var company = _cache.GetCachedCompanyAsync(_context, companyid).Result; //_context.Companies.Find(companyid);
            if (company == null)
                return false;
            var dateNow = DateTime.Now;
            DateTime min = dateNow.AddHours(-(company.OrderLeadTimeH.Value));
            dateNow = dateNow.AddDays(1);
            DateTime max = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 0, 0, 0);
            dateNow = dateNow.AddDays(1);

            //DateTime min = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 0, 0, 0);

            // dt = new DateTime(dt.Year, dt.Month, dt.Day, dateNow.Hour, dateNow.Minute, dateNow.Second);
            max = max.AddHours(company.OrderThresholdTimeH.HasValue ? company.OrderThresholdTimeH.Value : 24);
            //min = min.AddHours(-(company.OrderLeadTimeH.HasValue ? company.OrderLeadTimeH.Value : 24));

            // min = min.AddHours(-(company.OrderLeadTimeH.Value));
            //DateTime min = DateTime.Now.AddHours(-(company.OrderLeadTimeH.HasValue ? company.OrderLeadTimeH.Value : 24));
            if ((dt - min).TotalDays < 7)
                for (DateTime t = dt; t > min; t = t.AddDays(-1))
                {
                    if (t.DayOfWeek == DayOfWeek.Saturday || t.DayOfWeek == DayOfWeek.Sunday)
                    {
                        min = min.AddDays(-1);
                    }
                }
            //if (dt.Day == min.Day)
            //{
            //    return dt.TimeOfDay < min.TimeOfDay && dt < max;
            //}
            return dt > min && dt < max;
        }
        public CompanyModel GetOwnCompany(int companyid)
        {
            CompanyModel res;
            try
            {
                var company = _context.Hotel.Find(companyid);
                if (company == null)
                    throw new Exception("Company not exists");
                res = new CompanyModel()
                {
                    Name = company.Name,
                    Phone = company.Phone,
                    ZipCode = company.ZipCode,
                    Email = company.Email,

                    City = company.City,
                    Address1 = company.Address1,
                    Address2 = company.Address2,
                    Country = company.Country,
                    PictureId = company.PictureId,
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOwnCompany Company={0} ", companyid);
                return new CompanyModel(); //to do
            }
            return res;
        }
        public CompanyModel GetUserCompany(string UserId)
        {
            CompanyModel res;
            try
            {
                var user = _context.Users.Find(UserId);
                if (user == null)
                    throw new Exception("User not exists");
                res = new CompanyModel()
                {
                    Phone = user.PhoneNumber,
                    ZipCode = user.ZipCode,
                    Email = user.Email,
                    Name = user.UserName,
                    City = user.City,
                    Address1 = user.Address1,
                    Country = user.Country
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserCompany User={0} ", UserId);
                return new CompanyModel(); //to do
            }
            return res;
        }
        public IQueryable<UserDayDishViewModel> DishesPerDay(DateTime daydate, string userId, int companyid)
        {
            var query = from dish in _context.Dish
                        join dd in (from subday in _context.DayDish where subday.Date == daydate && subday.HotelId == companyid select subday) on dish.Id equals dd.DishId into proto
                        from dayd in proto.DefaultIfEmpty()

                        select new UserDayDishViewModel() { DishId = dish.Id, DishName = dish.Name, Date = daydate, Enabled = proto.Count() > 0/*dayd != null*/ };
            return query;
        }
        public IQueryable<UserDayDishViewModelPerGategory> CategorizedDishesPerDay(DateTime daydate, string userId, int companyid, int DishKindId)
        {
            var query1 = from dish in _context.Dish.Where(d => d.HotelId == companyid)
                         join dd in _context.DayDish.Where(d => d.HotelId == companyid && d.Date == daydate) on dish.Id equals dd.DishId
                         join ud in _context.UserDayDish.Where(ud => ud.HotelId == companyid && ud.UserId == userId && ud.Date == daydate )
                             on dish.Id equals ud.DishId into Details
                         from udayd in Details.DefaultIfEmpty()
                         select new UserDayDishViewModel
                         {
                             DishId = dish.Id,
                             CategoryId = dd.CategoriesId,
                             DishName = dish.Name,
                             MeasureUnit = dish.MeasureUnit,
                             DishKindId = DishKindId,
                             //Price=dish.Price,
                             DishDescription = dish.Description,
                             DishIngredientds = dish.Description,
                             //string.Join(",", from di in _context.DishIngredients.Where(t => t.DishId == dish.Id)
                             //                                    join ingr in _context.Ingredients on di.IngredientId equals ingr.Id
                             //                                    select ingr.Name),
                             PictureId = dish.PictureId,
                             Date = daydate,
                             Quantity = udayd.Date == daydate ? udayd.Quantity : 0,
                             Price = udayd.Date == daydate ? udayd.Price : dish.Price
                         };
            var ordered = OrderedDishesDay(daydate, userId, companyid);
            query1 = query1.Where(x => !ordered.Any(o => o.DishId == x.DishId));
            var query2 = from cat in _context.Categories
                         orderby cat.Code
                         select new UserDayDishViewModelPerGategory()
                         {
                             Date = daydate,
                             CategoryCode = cat.Code,
                             CategoryName = cat.Name,
                             UserDayDishes = from dd in query1.Where(q => q.CategoryId == cat.Id)
                                             select new UserDayDishViewModel()
                                             {
                                                 Date = dd.Date,
                                                 DishId = dd.DishId,
                                                 DishName = dd.DishName,
                                                 Price = dd.Price,
                                                 ReadyWeight = dd.ReadyWeight,
                                                 KKal = dd.KKal,
                                                 Quantity = dd.Quantity,
                                                 PictureId = dd.PictureId,
                                                 DishKindId = dd.DishKindId,
                                                 CategoryId = dd.CategoryId,
                                                 IsWeight = dd.IsWeight,
                                                 BaseWeight = dd.BaseWeight,
                                                 OrderBaseWeight = dd.OrderBaseWeight,
                                                 MinWeight = dd.MinWeight,
                                                 MeasureUnit = dd.MeasureUnit,
                                                 Enabled = dd.Enabled,
                                                 DishDescription = dd.DishDescription,
                                                 DishIngredientds = dd.DishIngredientds
                                             }
                         };
            query2 = query2.Where(cat => cat.UserDayDishes.Count() > 0);
            //List<UserDayDishViewModelPerGategory> res = new List<UserDayDishViewModelPerGategory>();
            //query2.ForEachAsync(c =>
            //{
            //    res.Add(c);
            //    c.UserDayDishes.ToList().ForEach(d =>
            //    {
            //        var ordDish = ordered.FirstOrDefault(ord => ord.CategoryId == d.CategoryId && ord.DishId == d.DishId && ord.DishKindId == DishKindId);
            //        if (ordDish != null)
            //        {
            //            res.Last().UserDayDishes.ToList().Remove(d);
            //        }
            //    });
            //});
            /* !! not more working on EF 3.0*/
            /*
            var query = from entry in   (
                            from dd in _context.DayDish
                            join d in _context.Dishes on dd.DishId equals d.Id
                            where dd.Date==daydate &&  dd.CompanyId == companyid
                            join ud in _context.UserDayDish on new { dd.DishId, dd.Date, uid=userId ,cid= companyid } equals new {ud.DishId, ud.Date,uid=ud.UserId, cid = ud.CompanyId } into proto
                            from userday in proto.DefaultIfEmpty()
                            select new { DishId = d.Id, CategoryID = d.CategoriesId, DishName = d.Name, Date = daydate, Enabled = proto.Count() > 0,  Quantity = proto.Count()>0? proto.First().Quantity :0}
                        )
                        group entry by entry.CategoryID into catgroup
                        join cat in _context.Categories on new { id=catgroup.Key, cid = companyid } equals new { id=cat.Id, cid =cat.CompanyId }
                        orderby cat.Code
                        select new UserDayDishViewModelPerGategory()
                        {
                            CategoryCode = cat.Code,
                            CategoryName = cat.Name,
                            UserDayDishes = from dentry in catgroup
                                            select new UserDayDishViewModel()
                                            {
                                                DishId = dentry.DishId,
                                                DishName = dentry.DishName,
                                                Date = dentry.Date,
                                                Enabled = dentry.Enabled,
                                                Quantity=dentry.Quantity
                                            }
                        };
           */
            return query2;

        }
        public IQueryable<UserDayDishViewModelPerGategory> CategorizedDishesPerDay(DateTime daydate, string userId, int companyid)
        {
            var query1 = from dish in _context.Dish.Where(d => d.HotelId == companyid)
                         join dd in _context.DayDish.Where(d => d.HotelId == companyid && d.Date == daydate) on dish.Id equals dd.DishId
                         //join dk in _context.DishesKind.WhereCompany(companyid) on dd.DishKindId equals dk.Id
                         join ud in _context.UserDayDish.Where(ud => ud.HotelId == companyid && ud.UserId == userId && ud.Date == daydate)
                             on dish.Id equals ud.DishId into Details
                         from udayd in Details.DefaultIfEmpty()
                         select new UserDayDishViewModel
                         {
                             UserId= userId,
                             DishId = dish.Id,
                             CategoryId = dd.CategoriesId,
                             DishName = dish.Name,
                             //IsWeight = dish.IsWeight,
                             //BaseWeight = dish.BaseWeight,
                             //OrderBaseWeight = dish.OrderBaseWeight,
                             //MinWeight = dish.MinWeight,
                             MeasureUnit = dish.MeasureUnit,
                             //DishKindId = udayd.DishKindId,
                             //DishKindName = dk.Name,
                             //Price=dish.Price,
                             DishDescription = dish.Description,
                             DishIngredientds = dish.Description,
                             PictureId = dish.PictureId,
                             Date = daydate,
                             Quantity = udayd.Date == daydate ? udayd.Quantity : 0,
                             Price = udayd.Date == daydate ? udayd.Price : dish.Price
                         };
            //var user = _userManager.FindByIdAsync(userId);
            //string userType = user.Result.GetUserType().ToString() + "Limit";
            //var limit = UserDayLimit(userId, companyid, userType, daydate).Result;
            var query2 = from cat in _context.Categories
                         orderby cat.Code
                         select new UserDayDishViewModelPerGategory()
                         {
                             Date = daydate,
                             CategoryId=cat.Id,
                             CategoryCode = cat.Code,
                             CategoryName = cat.Name,
                             //Limit = limit,
                             UserDayDishes = from dd in query1.Where(q => q.CategoryId == cat.Id)
                                             select new UserDayDishViewModel()
                                             {
                                                 UserId=dd.UserId,
                                                 Date = dd.Date,
                                                 DishId = dd.DishId,
                                                 DishName = dd.DishName,
                                                 Price = dd.Price,
                                                 ReadyWeight = dd.ReadyWeight,
                                                 KKal = dd.KKal,
                                                 Quantity = dd.Quantity,
                                                 PictureId = dd.PictureId,
                                                 DishKindId = dd.DishKindId,
                                                 DishKindName=dd.DishKindName,
                                                 CategoryId = dd.CategoryId,
                                                 IsWeight = dd.IsWeight,
                                                 BaseWeight = dd.BaseWeight,
                                                 OrderBaseWeight = dd.OrderBaseWeight,
                                                 MinWeight = dd.MinWeight,
                                                 MeasureUnit = dd.MeasureUnit,
                                                 Enabled = dd.Enabled,
                                                 DishDescription = dd.DishDescription,
                                                 DishIngredientds = dd.DishIngredientds
                                             }
                         };
            query2 = query2.Where(cat => cat.UserDayDishes.Count() > 0);
           
            return query2;

        }
        public DayDish SelectSingleOrDefault(int dishId, DateTime daydate)
        {
            return _context.DayDish.SingleOrDefault(dd => dd.DishId == dishId && dd.Date == daydate);
        }

        public IQueryable<CustomerOrdersViewModel> CustomerOrders(DateTime daydate, int companyid)
        {
            var query1 = from ud in _context.UserDay.Where(dd => dd.HotelId == companyid && dd.Date == daydate)
                         join user in _context.Users on ud.UserId equals user.Id
                         select new CustomerOrdersViewModel
                         {
                             UserId = ud.UserId,
                             UserName = "", //! todo
                             Date = daydate,
                             DishesCount = ud.Quantity,
                             Amount = ud.Total,
                             IsConfirmed = ud.IsConfirmed,
                             IsPaid = ud.IsPaid,
                             User = new CompanyModel()
                             {
                                 Phone = user.PhoneNumber,
                                 ZipCode = user.ZipCode,
                                 Email = user.Email,
                                 Name = user.UserName,
                                 City = user.City,
                                 Address1 = user.Address1,
                                 Country = user.Country
                             }

                         };


            return query1;


        }
        public CustomerOrdersViewModel CustomerOrders(string UserId, DateTime daydate, int companyid)
        {
            var query1 =
                           from dd in _context.DayDish.Where(dd => dd.HotelId == companyid && dd.Date == daydate)
                           join d in _context.Dish.Where(dd => dd.HotelId == companyid) on dd.DishId equals d.Id
                           join ud in _context.UserDayDish.Where(ud => ud.HotelId == companyid && ud.Date == daydate) on dd.DishId equals ud.DishId
                           join cu in _context.Users on ud.UserId equals cu.Id
                           select new CustomerOrdersDetailsViewModel
                           {
                               UserId = cu.Id,
                               UserName = cu.NormalizedUserName,
                               DishId = d.Id,
                               CategoryId = d.CategoriesId,
                               DishName = d.Name,
                               Date = daydate,
                               Quantity = ud.Quantity,
                               Price = d.Price,
                               Amount = ud.Quantity * d.Price
                           };
            var querysingle = query1.FirstOrDefault();
            var res = new CustomerOrdersViewModel()
            {

                Details = query1
            };
            if (querysingle != null)
            {
                res.UserId = querysingle.UserId;
                res.UserName = querysingle.UserName;
                res.Date = querysingle.Date;
            }


            return res;

        }

        public bool SaveDay(List<UserDayDish> daydishes, HttpContext httpcontext)
        {

            try
            {
                daydishes.ForEach(d =>
                {
                    //await saveday(d);
                    d.IsComplex = false;
                    httpcontext.User.AssignUserAttr(d);

                    var userDayDish = _context.UserDayDish.SingleOrDefault(c => c.HotelId == d.HotelId
                                && c.Date == d.Date
                                && c.UserId == d.UserId
                                && c.ComplexId == d.ComplexId);

                    if (userDayDish != null)
                    {
                        userDayDish.Quantity = d.Quantity;
                        userDayDish.Price = d.Price;
                        userDayDish.IsComplex = false;
                        _context.Update(userDayDish);
                    }
                    else if (d.Quantity > 0)
                    {
                        //d.UserId = this.User.GetUserId();

                        _context.Add(d);
                    }


                });
                //if (!UpdateUserDay(daydishes, httpcontext))
                //    return false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user day dish");
                return false;
            }
            return true;
        }

        private bool UpdateUserDay(List<UserDayDish> daydishes, HttpContext httpcontext)
        {
            var userid = httpcontext.User.GetUserId();
            var companyid = httpcontext.User.GetHotelID();
            bool isnew = false;
            UserDay userDay = null;

            if (daydishes.Count > 0)
            {
                DateTime daydate = daydishes.First().Date;
                userDay = _context.UserDay.FirstOrDefault(ud => ud.UserId == userid
                && ud.HotelId == companyid && ud.Date == daydate);
                if (userDay == null)
                {
                    isnew = true;
                    userDay = new UserDay() { Date = daydate };
                    httpcontext.User.AssignUserAttr(userDay);
                }
                userDay.Total = daydishes.Sum(d => d.Price * d.Quantity);

                userDay.Quantity = daydishes.Sum(d => d.Quantity);

            }
            try
            {
                if (isnew && userDay != null)
                {
                    _context.Add(userDay);
                }
                if (!isnew && userDay != null)
                {
                    _context.Update(userDay);
                }
                if (userDay != null)
                    _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user day ");
                return false;
            }
            return true;


        }

        public async Task<bool> SaveDayComplex(List<UserDayComplex> daycomplex, string userId, int companyId)
        {
            daycomplex.ForEach(d => { d.HotelId = companyId; d.UserId = userId; });
            try
            {
                daycomplex.ForEach(d =>
                {
                    //await saveday(d);
                    // httpcontext.User.AssignUserAttr(d);
                    if (d.Price == null)
                    {
                        return;
                    }
                    var userDayComplex = _context.UserDayComplex.SingleOrDefault(c => c.HotelId == d.HotelId
                                && c.Date == d.Date
                                && c.UserId == d.UserId
                                && c.ComplexId == d.ComplexId);
                    if (userDayComplex != null)
                    {
                        userDayComplex.Quantity = d.Quantity;
                        userDayComplex.Price = d.Price;
                        _context.Update(userDayComplex);
                    }
                    else if (d.Quantity > 0)
                    {
                        //d.UserId = this.User.GetUserId();

                        _context.Add(d);
                    }

                });
                _context.SaveChanges();
                //  if (!UpdateUserComplex(daycomplex, httpcontext))
                //     return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user day complex");
                return false;
            }
            return true;
        }

        public async Task<bool> SaveDayDishInComplex(List<UserDayDish> userDayDishes, string userId, int companyId)
        {
            var query = userDayDishes.GroupBy(x => new { x.ComplexId, x.DishId })
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            userDayDishes.ForEach(d => { d.HotelId = companyId; d.UserId = userId; });
            try
            {

                userDayDishes.ForEach(d =>
                {
                    //await saveday(d);
                    //httpcontext.User.AssignUserAttr(d);
                    var userDayDish = _context.UserDayDish.SingleOrDefault(c => c.HotelId == d.HotelId
                                && c.DishId == d.DishId
                                && c.Date == d.Date
                                && c.ComplexId == d.ComplexId
                                //&& c.DishKindId == d.DishKindId
                                && c.CategoriesId == d.CategoriesId
                                && c.UserId == d.UserId);
                    if (userDayDish != null)
                    {
                        userDayDish.Quantity = d.Quantity;
                        userDayDish.Price = d.Price;
                        _context.Update(userDayDish);
                        _logger.LogInformation("Update user day dish {1} {2} {3} {4}",d.UserId,d.Date,d.ComplexId,d.DishId);
                    }
                    else if (d.Quantity > 0)
                    {
                        //d.UserId = this.User.GetUserId();

                        _context.Add(d);
                        _logger.LogInformation("Add user day dish {1} {2} {3} {4}", d.UserId, d.Date, d.ComplexId, d.DishId);
                    }

                });
                _context.SaveChanges();
                //  if (!UpdateUserComplex(daycomplex, httpcontext))
                //     return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user day dish");
                return false;
            }

            return true;
        }
        public async Task<bool> SaveUserDay(int quantity, decimal total, decimal discount,int discountId, DateTime date, string userId, int companyId)
        {
            UserDay order = new UserDay();
            order.HotelId = companyId;
            order.Date = date;
            order.UserId = userId;
            order.Quantity = quantity;
            order.Total = total - discount;
            order.Discount = discount;
            order.DiscountId = discountId;
          //  order.IsUpdated = true;
            order.TotalWtithoutDiscount = total;
            order.IsConfirmed = true;

            try
            {

                //await saveday(d);
                // httpcontext.User.AssignUserAttr(d);
                var userDay = _context.UserDay.SingleOrDefault(c => c.HotelId == order.HotelId
                            && c.Date == order.Date
                            && c.UserId == order.UserId);
                if (userDay != null)
                {
                    userDay.Quantity += order.Quantity;
                    // userDay.Total += order.Total;
                    userDay.Total = total - discount;
                    userDay.TotalWtithoutDiscount = total;
                    userDay.Discount = discount;
                    userDay.IsConfirmed = true;
                    _context.Update(userDay);
                }
                else if (order.Quantity > 0)
                {
                    //d.UserId = this.User.GetUserId();

                    _context.Add(order);
                }


                _context.SaveChanges();
                //  if (!UpdateUserComplex(daycomplex, httpcontext))
                //     return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user day");
                return false;
            }
            return true;
        }
        public async Task<bool> UserFinanceEdit(decimal total, string userId, int companyId, bool add)
        {
            //            exec MakeOrderPayment '2020-08-07', 1
            //Select*
            //from UserFinOutComes
            DateTime date = DateTime.Now;
            try
            {

                //await saveday(d);
                // httpcontext.User.AssignUserAttr(d);
                var userFinance = _context.UserFinances.SingleOrDefault(c => c.HotelId == companyId && c.Id == userId);
                if (userFinance != null)
                {
                    userFinance.LastUpdated = date;
                    if (add)
                    {
                        userFinance.TotalPreOrderBalance += total;
                    }
                    else
                    {
                        userFinance.TotalPreOrderBalance -= total;
                    }
                    _context.Update(userFinance);
                }
                else
                {
                    //d.UserId = this.User.GetUserId();

                    //_context.Add(order);
                }


                _context.SaveChanges();
                //  if (!UpdateUserComplex(daycomplex, httpcontext))
                //     return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user finance");
                return false;
            }
            return true;
        }
        public async Task<bool> SaveComplexAndDishesDay(List<UserDayComplex> daycomplex, List<UserDayDish> userDayDishes, string userId, int companyId)
        {
            decimal total = 0;
            int quan = 0;
            daycomplex = daycomplex.Where(d => d.Quantity > 0).ToList();
            userDayDishes = userDayDishes.Where(d => d.Quantity > 0).ToList();
            daycomplex.ForEach(d =>
            {
                if (d.Quantity > 0)
                {
                    quan += d.Quantity;
                    total += d.Price * d.Quantity;
                }
            });
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //var discountplugin = _plugins.GetDiscointPlugin();
                decimal discount = 0;
                int discountId =0;
                var res = OrderedComplexDay(daycomplex.First().Date, userId, companyId).ToList();
                var orderedDishes = OrderedDishesDay(daycomplex.First().Date, userId, companyId).ToList();
                orderedDishes.ForEach(ord => {
                    total += ord.Price * ord.Quantity;
                    quan += ord.Quantity;
                });
                bool ordered = res.Any(x => daycomplex.Any(y => y.ComplexId == x.ComplexId));
                if (ordered)
                {
                    _logger.LogWarning("Already ordered complex in User Day {0} userId {1}", daycomplex.First().Date, userId);
                    return false;
                }
                res.ForEach(ord =>
                {
                    total += ord.Price * ord.Quantity;
                    daycomplex.Add(new UserDayComplex() { ComplexId = ord.ComplexId });
                });
                //if (discountplugin != null)
                //{
                //    daycomplex.ForEach(dc => { dc.Complex = _context.Complex.Find(dc.ComplexId); });
                //    //discountplugin.CalculateComplexDayDiscount(daycomplex, userDayDishes);
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex, companyId);
                // //   DiscountView dis = discountplugin.GetComplexDiscount(daycomplex, companyId);
                //    discount = dis.Amount;
                //    discountId = dis.Id;

                //}
                if (!await SaveDayComplex(daycomplex, userId, companyId))
                    return false;


                if (!await SaveDayDishInComplex(userDayDishes, userId, companyId))
                    return false;
                if (!await SaveUserDay(quan, total, discount,discountId, daycomplex.First().Date, userId, companyId))
                    return false;
                //if (!await UserFinanceEdit(total,userId, companyId,false))
                //    return false;
                scope.Complete();
            }
            return true;
        }
        //Delete ordered complex with dishes
        public async Task<bool> DeleteDayComplex(UserDayComplex userDayComplex, string userId, int companyId)
        {

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //var discountplugin = _plugins.GetDiscointPlugin();
                decimal discount = 0;
                int discountId = 0;
                decimal total = 0;
                var res = OrderedComplexDay(userDayComplex.Date, userId, companyId).ToList();
                var orderedDishes = OrderedDishesDay(userDayComplex.Date, userId, companyId).ToList();
                orderedDishes.ForEach(ord => {
                    total += ord.Price * ord.Quantity;
                });
                List<UserDayComplex> daycomplex = new List<UserDayComplex>();
                res.ForEach(ord =>
                {
                    if (userDayComplex.ComplexId != ord.ComplexId)
                    {
                        //total += ord.Price;
                        total += ord.Price * ord.Quantity;
                        daycomplex.Add(new UserDayComplex() { ComplexId = ord.ComplexId });
                    }
                });
                //if (discountplugin != null)
                //{
                //    daycomplex.ForEach(dc => { dc.Complex = _context.Complex.Find(dc.ComplexId); });
                //    //discountplugin.CalculateComplexDayDiscount(daycomplex, userDayDishes);
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex);
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex, companyId);
                //    DiscountView dis = discountplugin.GetComplexDiscount(daycomplex, companyId);
                //    discount = dis.Amount;
                //    discountId = dis.Id;
                //}
                if (!await DeleteDayComplexDb(userDayComplex, userId, companyId))
                    return false;


                if (!await DeleteDayDishInComplex(userDayComplex, userId, companyId))
                    return false;
                if (!await DeleteUserDay(total, discount,discountId, userDayComplex.Quantity, userDayComplex.Date, userId, companyId))
                    return false;
                //if (!await UserFinanceEdit(userDayComplex.Price, userId, companyId, true))
                //    return false;
                scope.Complete();
            }
            return true;
        }
        private async Task<bool> DeleteDayComplexDb(UserDayComplex userDayComplex, string userId, int companyId)
        {
            //var userId = httpcontext.User.GetUserId();
            //var companyId = httpcontext.User.GetCompanyID();
            try
            {
                var existing_db = await _context.UserDayComplex.Where
                    (di => di.ComplexId == userDayComplex.ComplexId &&
                    di.HotelId == companyId &&
                    di.UserId == userId &&
                    di.Date == userDayComplex.Date).ToListAsync();
                if (existing_db.Count() == 0)
                {
                    _logger.LogError("Delete UserDayComplex that doesn't exists {0}", userId);
                    return false;
                }
                _context.UserDayComplex.RemoveRange(existing_db);


                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete UserDayComplex");
                return false;
            }
            return true;
        }
        private async Task<bool> DeleteDayDishInComplex(UserDayComplex userDayComplex, string userId, int companyId)
        {
            //var userId = httpcontext.User.GetUserId();
            //var companyId = httpcontext.User.GetCompanyID();
            try
            {
                var existing_db = await _context.UserDayDish.Where
                    (di => di.ComplexId == userDayComplex.ComplexId &&
                    di.HotelId == companyId &&
                    di.UserId == userId &&
                    di.Date == userDayComplex.Date &&
                    di.IsComplex == true).ToListAsync();
                _context.UserDayDish.RemoveRange(existing_db);


                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete UserDayDish");
                return false;
            }
            return true;
        }
        private async Task<bool> DeleteUserDay(decimal total, decimal discount,int discountId, int quantity, DateTime date, string userId, int companyId)
        {
            //var userId = httpcontext.User.GetUserId();
            //var companyId = httpcontext.User.GetCompanyID();
            try
            {
                var existing_db = await _context.UserDay.Where
                    (di => di.HotelId == companyId &&
                    di.UserId == userId &&
                    di.Date == date).ToListAsync();
                if (existing_db.FirstOrDefault().Quantity > 1 && existing_db.FirstOrDefault().Quantity != quantity)
                {
                    var userDay = existing_db.FirstOrDefault();
                    if (userDay != null)
                    {
                        userDay.Quantity -= quantity;
                        userDay.Total = total - discount;
                        userDay.TotalWtithoutDiscount = total;
                        userDay.Discount = discount;
                        userDay.DiscountId = discountId;
                        _context.Update(userDay);
                    }
                }
                else
                {
                    _context.UserDay.RemoveRange(existing_db);
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete UserDayComplex");
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateDayComplex(UserDayComplex userDayComplex, string userId, int companyId, int newQuantity)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //var discountplugin = _plugins.GetDiscointPlugin();
                decimal discount = 0;
                int discountId = 0;
               
                decimal total = 0;
                var res = OrderedComplexDay(userDayComplex.Date, userId, companyId).ToList();
                var orderedDishes = OrderedDishesDay(userDayComplex.Date, userId, companyId).ToList();
                orderedDishes.ForEach(ord => {
                    total += ord.Price * ord.Quantity;
                });
                List<UserDayComplex> daycomplex = new List<UserDayComplex>();
                res.ForEach(ord =>
                {
                    if (userDayComplex.ComplexId != ord.ComplexId)
                    {
                        //total += ord.Price;
                        total += ord.Price * ord.Quantity;
                    }
                    else
                    {
                        total += ord.Price * newQuantity;
                    }
                    daycomplex.Add(new UserDayComplex() { ComplexId = ord.ComplexId });
                });
               

                //if (discountplugin != null)
                //{
                //    daycomplex.ForEach(dc => { dc.Complex = _context.Complex.Find(dc.ComplexId); });
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex, companyId);
                //    DiscountView dis = discountplugin.GetComplexDiscount(daycomplex, companyId);
                //    discount = dis.Amount;
                //    discountId = dis.Id;
                //}
                if (!await UpdateDayComplexDb(userDayComplex, userId, companyId, newQuantity))
                    return false;


                if (!await UpdateDayDishInComplex(userDayComplex, userId, companyId, newQuantity))
                    return false;
                if (!await UpdateUserDay(total, discount, userDayComplex.Quantity,newQuantity, userDayComplex.Date, userId, companyId))
                    return false;
                //if (!await UserFinanceEdit(userDayComplex.Price, userId, companyId, true))
                //    return false;
                scope.Complete();
            }
            return true;




        }
        public async Task<bool> UpdateDayComplexDb(UserDayComplex userDayComplex, string userId, int companyId, int newQuantity)
        {

            try
            {
                var userComplex = _context.UserDayComplex.SingleOrDefault(c => c.HotelId == companyId
                && c.UserId == userId
                && userDayComplex.ComplexId == c.ComplexId
                && userDayComplex.Date == c.Date);
                if (userComplex != null)
                {
                    userComplex.Quantity = newQuantity;
                    _context.Update(userComplex);
                }
                else
                {

                    _logger.LogError("Edit user Complex no Complex");
                    //_context.Add(order);
                    return false;
                }


                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit user Complex ");
                return false;
            }
            return true;
        }
        private async Task<bool> UpdateDayDishInComplex(UserDayComplex userDayComplex, string userId, int companyId, int newQuantity)
        {
            try
            {
                var userDishes = _context.UserDayDish.Where(c => c.HotelId == companyId
                 && c.UserId == userId
                 && userDayComplex.ComplexId == c.ComplexId
                 && c.IsComplex == true
                 && userDayComplex.Date == c.Date).ToList();
                if (userDishes.Count() != 0)
                {
                    userDishes.ForEach(d => { d.Quantity = newQuantity; });
                    _context.UpdateRange(userDishes);
                }
                else
                {

                    _logger.LogError("Edit user Dish no Complex");
                    //_context.Add(order);
                    return false;
                }


                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit user Dish ");
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateUserDay(decimal total, decimal discount, int oldQuantity,int newQuantity, DateTime date, string userId,int companyId)
        {

            try
            {
                var userDay = _context.UserDay.SingleOrDefault(c => c.HotelId == companyId
                && c.UserId == userId
                && date == c.Date);
                if (userDay != null)
                {
                    userDay.Total = total-discount;
                    userDay.Discount = discount;
                    userDay.Quantity = userDay.Quantity -oldQuantity+ newQuantity;
                    userDay.TotalWtithoutDiscount = total;
                    _context.Update(userDay);
                }
                else
                {

                    _logger.LogError("Edit user Day no order");
                    //_context.Add(order);
                    return false;
                }


                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit user Day ");
                return false;
            }
            return true;
        }
        public async Task<bool> SaveDishesDay(List<UserDayDish> userDayDishes, string userId, int companyId)
        {
            decimal total = 0;
            int quan = 0;
            userDayDishes = userDayDishes.Where(d => d.Quantity > 0).ToList();
            //if (userDayDishes.Count() == 0)
            //{
            //    return true;
            //}
            userDayDishes.ForEach(d =>
            {
                if (d.Quantity > 0)
                {
                    quan += d.Quantity;
                    total += d.Price * d.Quantity;
                }
            });
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
               
                var res = OrderedDishesDay(userDayDishes.First().Date, userId, companyId).ToList();
                bool ordered = res.Any(x => userDayDishes.Any(y => y.DishId == x.DishId /*&& y.DishKindId==x.DishKindId*/&&y.CategoriesId==x.CategoryId));
                if (ordered)
                {
                    _logger.LogWarning("Already ordered dish in User Day {0} userId {1}", userDayDishes.First().Date, userId);
                    return false;
                }
                //res.ForEach(ord =>
                //{
                //    total += ord.Price * ord.Quantity;
                //    quan += ord.Quantity;
                //});
                


                if (!await SaveDayDish(userDayDishes, userId, companyId))
                    return false;
                if (!await SaveUserDayWithoutDiscount(quan, total,userDayDishes.First().Date, userId, companyId))
                    return false;
                scope.Complete();
            }
            return true;
        }
        public async Task<bool> SaveDayDish(List<UserDayDish> userDayDishes, string userId, int companyId, bool isDelivered=false)
        {
            
            userDayDishes.ForEach(d => { d.HotelId = companyId; d.UserId = userId; });
            try
            {

                userDayDishes.ForEach(d =>
                {
                    //await saveday(d);
                    //httpcontext.User.AssignUserAttr(d);
                    d.ComplexId = 0;
                    d.IsDelivered = isDelivered;
                    var userDayDish = _context.UserDayDish.SingleOrDefault(c => c.HotelId == d.HotelId
                                && c.DishId == d.DishId
                                && c.Date == d.Date
                                //&& c.DishKindId==d.DishKindId
                                && c.CategoriesId == d.CategoriesId
                                && c.ComplexId == d.ComplexId
                                && c.UserId == d.UserId);
                    if (userDayDish != null)
                    {
                        userDayDish.Quantity += d.Quantity;
                        userDayDish.Price = d.Price;
                        userDayDish.IsDelivered = isDelivered;
                        _context.Update(userDayDish);
                    }
                    else if (d.Quantity > 0)
                    {
                        //d.UserId = this.User.GetUserId();

                        _context.Add(d);
                    }

                });
                _context.SaveChanges();
                //  if (!UpdateUserComplex(daycomplex, httpcontext))
                //     return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save user day dish");
                return false;
            }

            return true;
        }
        public async Task<bool> SaveUserDayWithoutDiscount(int quantity, decimal total,DateTime date, string userId, int companyId,bool isUpdated=false,bool isPaid=false)
        {
            UserDay order = new UserDay();
            order.HotelId = companyId;
            order.Date = date;
            order.UserId = userId;
            order.Quantity = quantity;
            order.Total = total;
            order.TotalWtithoutDiscount = total;
            order.IsConfirmed = true;
            order.IsUpdated = isUpdated;
            if (isUpdated)
            {
                order.TotalUserOrder = total;
            }
            order.IsPaid = isPaid;
            try
            {

                //await saveday(d);
                // httpcontext.User.AssignUserAttr(d);
                var userDay = _context.UserDay.SingleOrDefault(c => c.HotelId == order.HotelId
                            && c.Date == order.Date
                            && c.UserId == order.UserId);
                if (userDay != null)
                {
                    userDay.Quantity += order.Quantity;
                    // userDay.Total += order.Total;
                    userDay.IsUpdated = isUpdated;
                    userDay.Total += total;
                    userDay.TotalWtithoutDiscount +=total;
                    if (isUpdated)
                    {
                        userDay.TotalUserOrder += total;
                    }
                    userDay.IsConfirmed = true;
                    _context.Update(userDay);
                }
                else if (order.Quantity > 0)
                {
                    //d.UserId = this.User.GetUserId();
                    order.Discount = 0;
                    _context.Add(order);
                }


                _context.SaveChanges();
                //  if (!UpdateUserComplex(daycomplex, httpcontext))
                //     return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user day (withoutDiscount)");
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteDayDish(UserDayDish userDayDish, string userId, int companyId)
        {

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //var discountplugin = _plugins.GetDiscointPlugin();
                decimal discount = 0;
                int discountId = 0;
                decimal total = 0;
                var res = OrderedComplexDay(userDayDish.Date, userId, companyId).ToList();
                var orderedDishes = OrderedDishesDay(userDayDish.Date, userId, companyId).ToList();
                orderedDishes.ForEach(ord => {
                    if (ord.DishId == userDayDish.DishId && ord.CategoryId == userDayDish.CategoriesId /*&& ord.DishKindId == userDayDish.DishKindId*/)
                    {
                    }
                    else { 
                        total += ord.Price * ord.Quantity;
                    }
                });
                List<UserDayComplex> daycomplex = new List<UserDayComplex>();
                res.ForEach(ord =>
                {
                        total += ord.Price * ord.Quantity;
                        daycomplex.Add(new UserDayComplex() { ComplexId = ord.ComplexId });
                    
                });
                //if (discountplugin != null)
                //{
                //    daycomplex.ForEach(dc => { dc.Complex = _context.Complex.Find(dc.ComplexId); });
                //    //discountplugin.CalculateComplexDayDiscount(daycomplex, userDayDishes);
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex);
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex, companyId);
                //    DiscountView dis = discountplugin.GetComplexDiscount(daycomplex, companyId);
                //    discount = dis.Amount;
                //    discountId = dis.Id;
                //}
               
                if (!await DeleteDayDishOutComplex(userDayDish, userId, companyId))
                    return false;
                if (!await DeleteUserDay(total, discount, discountId, userDayDish.Quantity, userDayDish.Date, userId, companyId))
                    return false;
                //if (!await UserFinanceEdit(userDayComplex.Price, userId, companyId, true))
                //    return false;
                scope.Complete();
            }
            return true;
        }
        private async Task<bool> DeleteDayDishOutComplex(UserDayDish userDayDish, string userId, int companyId)
        {
            //var userId = httpcontext.User.GetUserId();
            //var companyId = httpcontext.User.GetCompanyID();
            try
            {
                var existing_db = await _context.UserDayDish.Where
                    (di => di.DishId == userDayDish.DishId &&
                    di.HotelId == companyId &&
                    di.UserId == userId &&
                    di.Date == userDayDish.Date &&
                    //di.DishKindId==userDayDish.DishKindId&&
                    di.CategoriesId==userDayDish.CategoriesId&&
                    di.IsComplex == false).ToListAsync();
                if (existing_db.Count() == 0)
                {
                    _logger.LogError("Delete UserDayDishOutComplex that doesn't exists {0}", userId);
                    return false;
                }
                _context.UserDayDish.RemoveRange(existing_db);


                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete UserDayDishOutComplex");
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateDayDish(UserDayDish userDayDish, string userId, int companyId, int newQuantity)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                //var discountplugin = _plugins.GetDiscointPlugin();
                decimal discount = 0;
                int discountId = 0;

                decimal total = 0;
                var res = OrderedComplexDay(userDayDish.Date, userId, companyId).ToList();
                var orderedDishes = OrderedDishesDay(userDayDish.Date, userId, companyId).ToList();
                orderedDishes.ForEach(ord => {
                    if (ord.DishId == userDayDish.DishId && ord.CategoryId == userDayDish.CategoriesId /*&& ord.DishKindId == userDayDish.DishKindId*/)
                    {
                        total += ord.Price * newQuantity;
                    }
                    else
                    {
                        total += ord.Price * ord.Quantity;
                    }
                });
                List<UserDayComplex> daycomplex = new List<UserDayComplex>();
                res.ForEach(ord =>
                {
                    
                        total += ord.Price * ord.Quantity;
                    
                    daycomplex.Add(new UserDayComplex() { ComplexId = ord.ComplexId });
                });


                //if (discountplugin != null)
                //{
                //    daycomplex.ForEach(dc => { dc.Complex = _context.Complex.Find(dc.ComplexId); });
                //    //discount = discountplugin.GetComplexDayDiscount(daycomplex, companyId);
                //    DiscountView dis = discountplugin.GetComplexDiscount(daycomplex, companyId);
                //    discount = dis.Amount;
                //    discountId = dis.Id;
                //}
                if (!await UpdateUserDayDish(userDayDish, userId, companyId, newQuantity))
                    return false;

                if (!await UpdateUserDay(total, discount, userDayDish.Quantity, newQuantity, userDayDish.Date, userId, companyId))
                    return false;
                //if (!await UserFinanceEdit(userDayComplex.Price, userId, companyId, true))
                //    return false;
                scope.Complete();
            }
            return true;




        }
        private async Task<bool> UpdateUserDayDish(UserDayDish userDayDish, string userId, int companyId, int newQuantity)
        {
            try
            {
                var userDishes = _context.UserDayDish.Where(c => c.HotelId == companyId
                 && c.UserId == userId
                 && userDayDish.DishId == c.DishId&&
                 c.Date == userDayDish.Date &&
                 //c.DishKindId == userDayDish.DishKindId &&
                 c.CategoriesId == userDayDish.CategoriesId &&
                 c.IsComplex == false).ToList();
                if (userDishes.Count() != 0)
                {
                    userDishes.ForEach(d => { d.Quantity = newQuantity; });
                    _context.UpdateRange(userDishes);
                }
                else
                {

                    _logger.LogError("Update user Dish  no Dishes found");
                    //_context.Add(order);
                    return false;
                }


                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user Dish ");
                return false;
            }
            return true;
        }
        public async Task<bool> AddDishesDay(List<UserDayDish> userDayDishes, string userId, int companyId)
        {
            decimal total = 0;
            int quan = 0;
            userDayDishes = userDayDishes.Where(d => d.Quantity > 0).ToList();
            //if (userDayDishes.Count() == 0)
            //{
            //    return true;
            //}
            userDayDishes.ForEach(d =>
            {
                if (d.Quantity > 0)
                {
                    quan += d.Quantity;
                    total += d.Price * d.Quantity;
                }
            });
            string orderId = Guid.NewGuid().ToString();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!await SaveDayDish(userDayDishes, userId, companyId,true))
                    return false;
                if (!await SaveUserDayWithoutDiscount(quan, total, userDayDishes.First().Date, userId, companyId,true,true))
                    return false;
                //if (!await AddUserOrder(orderId,userDayDishes, userDayDishes.First().Date, userId, companyId,quan, total, true))
                //    return false;
                //if (!await MakeOrderPaymentAsync(userDayDishes.First().Date, companyId, userId, orderId))
                //    return false;
                scope.Complete();
            }
            return true;
        }
        //public async Task<bool> AddUserOrder(string orderId, List<UserDayDish> userDayDishes, DateTime date, string userId, int companyId,int quan,decimal total, bool isDelivered = false)
        //{
        //    UserOrder order = new UserOrder();
        //    order.Id = orderId;
        //    order.CompanyId = companyId;
        //    order.Quantity = quan;
        //    order.Date = date+DateTime.Now.TimeOfDay;
        //    order.UserId = userId;
        //    order.Total = total;
        //    order.IsDelivered = isDelivered;
        //    List <UserOrderDish> orderDishes = new List<UserOrderDish>();
        //    userDayDishes.ForEach(udd => {
        //        orderDishes.Add(new UserOrderDish
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            CompanyId = companyId,
        //            DishId = udd.DishId,
        //            Price = udd.Price,
        //            Quantity = udd.Quantity,
        //            OrderId = order.Id
        //        }) ;
        //    });
        //    try
        //    {
        //        _context.Add(order);
        //        _context.AddRange(orderDishes);
        //        _context.SaveChanges();
               
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "AddUserOrder");
        //        return false;
        //    }
        //    return true;
        //}
        
        /*   old , to be deleted further */
        public IQueryable<CustomerOrdersViewModel> CustomerOrders_old(DateTime daydate, int companyid)
        {
            var query1 =
                           from dd in _context.DayDish.Where(dd => dd.HotelId == companyid && dd.Date == daydate)
                           join d in _context.Dish.Where(dd => dd.HotelId == companyid) on dd.DishId equals d.Id
                           join ud in _context.UserDayDish.Where(ud => ud.HotelId == companyid && ud.Date == daydate) on dd.DishId equals ud.DishId
                           join cu in _context.Users on ud.UserId equals cu.Id
                           select new CustomerOrdersDetailsViewModel
                           {
                               UserId = cu.Id,
                               UserName = cu.NormalizedUserName,
                               DishId = d.Id,
                               CategoryId = d.CategoriesId,
                               DishName = d.Name,
                               Date = daydate,
                               Quantity = ud.Quantity,
                               Price = d.Price,
                               Amount = ud.Quantity * d.Price
                           };
            var query2 = from entry in query1
                         group entry by entry.UserId into grp
                         select new CustomerOrdersViewModel
                         {
                             UserId = grp.Key,
                             UserName = grp.Min(a => a.UserName), //! todo
                             Date = daydate,
                             DishesCount = grp.Count(),
                             Amount = grp.Sum(a => a.Amount)

                         };

            /*
                        var query = from entry in (
                                       from dd in _context.DayDish
                                       join d in _context.Dishes on dd.DishId equals d.Id
                                       where dd.Date == daydate && dd.CompanyId == companyid
                                       join ud in _context.UserDayDish on new { dd.DishId, dd.Date, cid = companyid } equals new { ud.DishId, ud.Date, cid = ud.CompanyId }
                                       join cu in _context.Users on ud.UserId equals cu.Id
                                       select new { UserId = cu.Id, UserName = cu.NormalizedUserName, DishId = d.Id, CategoryID = d.CategoriesId, DishName = d.Name, Date = daydate, ItemQuanity = ud.Quantity, ItemPrice = d.Price, ItemAmount = ud.Quantity * d.Price }
                                       )
                                    group entry by entry.UserId into ordergroup
                                  //  join cat in _context.Categories on new { id = ordergroup.First().CategoryID, cid = companyid } equals new { id = cat.Id, cid = cat.CompanyId }

                                    select new CustomerOrdersViewModel
                                    {
                                        UserId= ordergroup.Key,
                                        UserName = ordergroup.First().UserName,
                                      //  Date = daydate,
                                      //  DishesCount = ordergroup.Count(),
                                      //  Amount= ordergroup.Sum(a=>a.ItemAmount)

                                    };
                */
            return query2;

        }
        public IQueryable<UserDayComplexViewModel> ComplexPerDay(DateTime daydate, string userId, int companyid)
        {
            var query = from comp in _context.Complex

                        join dc in (from subday in _context.DayComplex where subday.Date == daydate && subday.HotelId == companyid select subday) on comp.Id equals dc.ComplexId
                        join dd in (from usubday in _context.UserDayComplex where usubday.UserId == userId && usubday.Date == daydate && usubday.HotelId == companyid select usubday) on dc.ComplexId equals dd.ComplexId into proto
                        from dayd in proto.DefaultIfEmpty()

                        select new UserDayComplexViewModel()
                        {
                            ComplexId = comp.Id,
                            ComplexName = comp.Name,
                            Quantity = dayd.Quantity,
                            Price = comp.Price,
                            Date = daydate,
                            Enabled = dayd.Date == daydate,  /*dayd != null*/
                            ComplexDishes = from d in _context.Dish.WhereCompany(companyid)
                                            join dc in _context.DishComplex.WhereCompany(companyid) on d.Id equals dc.DishId
                                            //join udd in _context.UserDayDish.WhereCompany(companyid).Where(i => i.Date == daydate && i.UserId == userId)  on d.Id equals udd.DishId
                                            where dc.ComplexId == comp.Id
                                            orderby dc.DishCourse
                                            select new UserDayComplexDishViewModel()
                                            {

                                                DishId = d.Id,
                                                DishName = d.Name,
                                                DishReadyWeight = d.ReadyWeight,
                                                PictureId = d.PictureId,
                                                DishCourse = dc.DishCourse,
                                                //  DishQuantity = udd.Quantity,

                                                DishDescription = d.Description,
                                                //DishIngredients = string.Join(",", from di in _context.DishIngredients.WhereCompany(companyid).Where(t => t.DishId == d.Id)
                                                //                                   join ingr in _context.Ingredients on di.IngredientId equals ingr.Id
                                                //                                   select ingr.Name),
                                            }
                        };
            return query;
        }




        public IQueryable<UserDayComplexViewModel> AvaibleComplexDay(DateTime daydate, string userId, int companyid)
        {
            var ordered = OrderedComplexDay(daydate, userId, companyid);
            var orderedList = ordered.ToList();
            var query = from comp in _context.Complex
                        join dc in (from subday in _context.DayComplex where subday.Date == daydate && subday.HotelId == companyid select subday) on comp.Id equals dc.ComplexId
                        join cat in _context.Categories.WhereCompany(companyid) on comp.CategoriesId equals cat.Id
                        //join dk in _context.DishesKind on comp.DishKindId equals dk.Id into leftdk
                        //from subdk in leftdk.DefaultIfEmpty()
                        select new UserDayComplexViewModel()
                        {
                            ComplexId = comp.Id,
                            ComplexName = comp.Name,
                            ComplexCategoryId = cat.Id,
                            ComplexCategoryName = cat.Name,
                            ComplexCategoryCode = cat.Code,
                           // DishKindId = comp.DishKindId,
                            Quantity = 0,
                            Price = comp.Price,
                            Date = daydate,
                            Enabled = dc.Date == daydate,  /*dayd != null*/
                            ComplexDishes = from d in _context.Dish.WhereCompany(companyid)
                                            join dishCom in _context.DishComplex.WhereCompany(companyid) on d.Id equals dishCom.DishId
                                            //join udd in _context.UserDayDish.WhereCompany(companyid).Where(i => i.Date == daydate && i.UserId == userId)  on d.Id equals udd.DishId
                                            where dishCom.ComplexId == comp.Id
                                            orderby dishCom.DishCourse ascending, dishCom.IsDefault descending
                                            select new UserDayComplexDishViewModel()
                                            {

                                                DishId = d.Id,
                                                DishName = d.Name,
                                                DishReadyWeight = d.ReadyWeight,
                                                PictureId = d.PictureId,
                                                DishCourse = dishCom.DishCourse,
                                                IsDefault = dishCom.IsDefault,
                                                //  DishQuantity = udd.Quantity,

                                                DishDescription = d.Description,
                                                DishIngredients = ""/* string.Join(",", from di in _context.DishIngredients.WhereCompany(companyid).Where(t => t.DishId == d.Id)
                                                                                   join ingr in _context.Ingredients on di.IngredientId equals ingr.Id
                                                                                   select ingr.Name)*/
                                            }
                        };
            query = query.Where(x => !ordered.Any(o => o.ComplexCategoryId == x.ComplexCategoryId));
            //foreach (var item in ordered) {
            //    query = query.Where(x => x.ComplexCategoryId != item.ComplexCategoryId);
            //        }
            return query;
        }
        public IQueryable<UserDayComplexViewModel> AvaibleComplexDayForMany(DateTime daydate, string userId, int companyid)
        {
            var ordered = OrderedComplexDay(daydate, userId, companyid);
            var orderedList = ordered.ToList();
            var query = from comp in _context.Complex
                        join dc in (from subday in _context.DayComplex where subday.Date == daydate && subday.HotelId == companyid select subday) on comp.Id equals dc.ComplexId
                        join cat in _context.Categories.WhereCompany(companyid) on comp.CategoriesId equals cat.Id
                        //join dk in _context.DishesKind on comp.DishKindId equals dk.Id into leftdk
                        //from subdk in leftdk.DefaultIfEmpty()
                        select new UserDayComplexViewModel()
                        {
                            ComplexId = comp.Id,
                            ComplexName = comp.Name,
                            ComplexCategoryId = cat.Id,
                            ComplexCategoryName = cat.Name,
                            ComplexCategoryCode = cat.Code,
                            DishKindId = comp.DishKindId,
                            Quantity = 0,
                            Price = comp.Price,
                            Date = daydate,
                            Enabled = dc.Date == daydate,  /*dayd != null*/
                            ComplexDishes = from d in _context.Dish.WhereCompany(companyid)
                                            join dishCom in _context.DishComplex.WhereCompany(companyid) on d.Id equals dishCom.DishId
                                            //join udd in _context.UserDayDish.WhereCompany(companyid).Where(i => i.Date == daydate && i.UserId == userId)  on d.Id equals udd.DishId
                                            where dishCom.ComplexId == comp.Id
                                            orderby dishCom.DishCourse ascending, dishCom.IsDefault descending
                                            select new UserDayComplexDishViewModel()
                                            {

                                                DishId = d.Id,
                                                DishName = d.Name,
                                                DishReadyWeight = d.ReadyWeight,
                                                PictureId = d.PictureId,
                                                DishCourse = dishCom.DishCourse,
                                                IsDefault = dishCom.IsDefault,
                                                //  DishQuantity = udd.Quantity,

                                                DishDescription = d.Description,
                                                DishIngredients = ""/* string.Join(",", from di in _context.DishIngredients.WhereCompany(companyid).Where(t => t.DishId == d.Id)
                                                                                   join ingr in _context.Ingredients on di.IngredientId equals ingr.Id
                                                                                   select ingr.Name)*/
                                            }
                        };
            query = query.Where(x => !ordered.Any(o => o.ComplexId == x.ComplexId));
            //foreach (var item in ordered) {
            //    query = query.Where(x => x.ComplexCategoryId != item.ComplexCategoryId);
            //        }
            return query;
        }
        public WeekReportModel WeekOrder(DateTime dayFrom, DateTime dayTo, string userId, int companyid)
        {
            WeekReportModel res = new WeekReportModel();
            res.Buyer = GetUserCompany(userId);
            res.Seller = GetOwnCompany(companyid);
            List<DayReportModel> reports = new List<DayReportModel>();
           
            var avaibleComplexDays = from comp in _context.DayComplex.Where(c =>c.Date >= dayFrom && c.Date <= dayTo && c.HotelId == companyid)
                                     //join dish in _context.DayDish.Where(c => c.Date >= dayFrom && c.Date <= dayTo && c.CompanyId == companyid) 
                                     select new UserDayComplexViewModel()
                                     {
                                         Date = comp.Date,
                                         Enabled = false
                                     };
            var avaibleDishesDays = from dish in _context.DayDish.Where(c => c.Date >= dayFrom && c.Date <= dayTo && c.HotelId == companyid)
                                    select new UserDayComplexViewModel()
                                    {
                                        Date = dish.Date,
                                        Enabled = false
                                    };
            var avaibleDays = avaibleComplexDays.Union(avaibleDishesDays).ToList();
            var orderedComplexs = from comp in _context.Complex
                                  join cat in _context.Categories.WhereCompany(companyid) on comp.CategoriesId equals cat.Id
                                  //join uday in _context.UserDay.Where(ud => ud.CompanyId == companyid & ud.Date >= dayFrom && ud.Date <= dayTo && ud.UserId == userId) on comp.CompanyId equals uday.CompanyId
                                  join dd in (from usubday in _context.UserDayComplex where usubday.UserId == userId && usubday.Date >= dayFrom && usubday.Date <= dayTo && usubday.HotelId == companyid select usubday) on comp.Id equals dd.ComplexId into proto
                          from dayd in proto.DefaultIfEmpty()
                          join uday in _context.UserDay.Where(ud => ud.HotelId == companyid & ud.Date >= dayFrom && ud.Date <= dayTo && ud.UserId == userId) on dayd.Date equals uday.Date
                          where dayd.Quantity > 0
                          select new UserDayComplexViewModel()
                          {
                              ComplexId = comp.Id,
                              ComplexName = comp.Name,
                              ComplexCategoryId = cat.Id,
                              ComplexCategoryName = cat.Name,
                              Quantity = dayd.Quantity,
                              Price = comp.Price,
                              Date = dayd.Date,
                              Total=uday.Total,
                              TotalWithoutDiscount=uday.TotalWtithoutDiscount,
                              Discount = uday.Discount,
                              Enabled = true,  /*dayd != null*/
                              ComplexDishes = from d in _context.Dish.WhereCompany(companyid)
                                                  //join dc in _context.DishComplex.WhereCompany(companyid) on d.Id equals dc.DishId
                                              join udd in _context.UserDayDish.WhereCompany(companyid).Where(i => i.Date >= dayFrom && i.Date <= dayTo && i.UserId == userId && i.ComplexId == comp.Id) on d.Id equals udd.DishId
                                              where udd.ComplexId == comp.Id
                                              //   orderby dc.DishCourse
                                              select new UserDayComplexDishViewModel()
                                              {

                                                  DishId = d.Id,
                                                  DishName = d.Name,
                                                  DishReadyWeight = d.ReadyWeight,
                                                  PictureId = d.PictureId,
                                                  // DishCourse = dc.DishCourse,
                                                  DishQuantity = udd.Quantity,

                                                  DishDescription = d.Description,
                                                  DishIngredients = d.Description,
                                                  //string.Join(",", from di in _context.DishIngredients.WhereCompany(companyid).Where(t => t.DishId == d.Id)
                                                  //                                   join ingr in _context.Ingredients on di.IngredientId equals ingr.Id
                                                  //                                   select ingr.Name),
                                              }
                          };
            var orderedDishes = from dish in _context.Dish
                        
                        join dd in (from usubday in _context.UserDayDish where usubday.UserId == userId && usubday.Date >= dayFrom && usubday.Date <= dayTo && usubday.HotelId == companyid && usubday.IsComplex == false select usubday) on dish.Id equals dd.DishId into proto
                        from dayd in proto.DefaultIfEmpty()
                        join cat in _context.Categories.WhereCompany(companyid) on dayd.CategoriesId equals cat.Id
                                join uday in _context.UserDay.Where(ud => ud.HotelId == companyid & ud.Date >= dayFrom && ud.Date <= dayTo && ud.UserId == userId) on dayd.Date equals uday.Date
                                where dayd.Quantity > 0
                        orderby cat.Code
                        select new UserDayDishViewModel()
                        {
                            DishId = dish.Id,
                            DishName = dish.Name,
                            DishDescription = dish.Description,
                            CategoryId = cat.Id,
                            //DishKindId = dayd.DishKindId,
                            //OrderBaseWeight = dayd.Base,
                            CategoryName = cat.Name,
                            CategoryCode = cat.Code,
                            //IsWeight = dish.IsWeight,
                            Quantity = dayd.Quantity,
                            Price = dayd.Price,
                            Date = dayd.Date,
                            PictureId = dish.PictureId,
                            Confirmed = uday.IsConfirmed,
                            Total = uday.Total,
                            //TotalWithoutDiscount = uday.TotalWtithoutDiscount,
                            //Discount = uday.Discount,
                            Enabled = true,  /*dayd != null*/

                        };
            avaibleDays.ForEach(dayAv =>
            {
                DayReportModel dayRep = new DayReportModel() { Date = dayAv.Date,Enabled=false };
                
                var categories = orderedComplexs.Where(o => o.Date == dayAv.Date).Select(ord => ord.ComplexCategoryId)
               .Union(orderedDishes.Where(l=> l.Date== dayAv.Date).Select(ord => ord.CategoryId)).ToList();

                var complexes = orderedComplexs.Where(i => i.Date == dayAv.Date);
                var dishes = orderedDishes.Where(j => j.Date == dayAv.Date);
                List<UserOrderedDay> dishesPerCategory = new List<UserOrderedDay>();
                
                categories.ForEach(cat =>
                {
                    UserOrderedDay day = new UserOrderedDay();
                    day.Date = day.Date;
                    day.CategoryId = cat;
                    
                    if (complexes.Where(c => c.ComplexCategoryId == cat).ToList().Count() == 0)
                    {
                        day.CategoryName = dishes.Where(com => com.CategoryId == cat).ToList().First().CategoryName;
                        day.CategoryCode = dishes.Where(com => com.CategoryId == cat).ToList().First().CategoryCode;
                    }
                    else
                    {
                        //var categ = complexes.Where(com => com.ComplexCategoryId == cat).ToList().First();
                        day.CategoryName = complexes.Where(com => com.ComplexCategoryId == cat).ToList().First().ComplexCategoryName;
                        day.CategoryCode = complexes.Where(com => com.ComplexCategoryId == cat).ToList().First().ComplexCategoryCode;
                    }
                    day.UserDayComplex = complexes.Where(c => c.ComplexCategoryId == cat).ToList();
                    day.UserDayDish = dishes.Where(c => c.CategoryId == cat).ToList();
                    if (complexes.ToList().Count() != 0)
                    {
                        day.Total = complexes.ToList().First().Total;
                        day.TotalWithoutDiscount = complexes.ToList().First().TotalWithoutDiscount;
                        day.DiscountSum = complexes.ToList().First().Discount;
                    }
                    else
                    {
                        day.Total = dishes.ToList().First().Total;
                        day.TotalWithoutDiscount = dishes.ToList().First().Total;
                        day.DiscountSum = 0;
                    }
                    dishesPerCategory.Add(day);
                });
                if (dishesPerCategory.Count() > 0)
                {
                    dayRep.Enabled = true;
                    dayRep.Total = dishesPerCategory.First().Total;
                    //dayRep.TotalWithoutDiscount = dishesPerCategory.Sum(tot => tot.TotalWithoutDiscount!=null ? (decimal)tot.TotalWithoutDiscount : 0);
                    //dayRep.TotalWithoutDiscount = dishesPerCategory.Sum(tot => tot.DiscountSum != null ? (decimal)tot.DiscountSum : 0);
                    dayRep.TotalWithoutDiscount = dishesPerCategory.First().TotalWithoutDiscount != null ? (decimal)dishesPerCategory.First().TotalWithoutDiscount : dayRep.Total;
                    dayRep.Discount = dishesPerCategory.First().DiscountSum != null ? (decimal)dishesPerCategory.First().DiscountSum : 0;
                }
                dayRep.DishesPerCategory = dishesPerCategory;
                reports.Add(dayRep);
               
            });
            res.Items = reports;
         
            return res;
        }
        public IQueryable<UserDayComplexViewModel> OrderedComplexDay(DateTime daydate, string userId, int companyid)
        {
            var confirmed = from ud in _context.UserDay
                            where ud.HotelId == companyid & ud.Date == daydate && ud.UserId == userId
                            select new UserDay();
            var query = from comp in _context.Complex
                            // join udd in (from subday in _context.UserDayDish where subday.Date == daydate && subday.CompanyId == companyid select subday) on comp.Id equals udd.ComplexId
                        join cat in _context.Categories.WhereCompany(companyid) on comp.CategoriesId equals cat.Id
                        join uday in _context.UserDay.Where(ud => ud.HotelId == companyid & ud.Date == daydate && ud.UserId == userId) on comp.HotelId equals uday.HotelId
                        join dd in (from usubday in _context.UserDayComplex where usubday.UserId == userId && usubday.Date == daydate && usubday.HotelId == companyid select usubday) on comp.Id equals dd.ComplexId into proto
                        from dayd in proto.DefaultIfEmpty()
                        where dayd.Quantity > 0
                        orderby cat.Code
                        select new UserDayComplexViewModel()
                        {
                            ComplexId = comp.Id,
                            ComplexName = comp.Name,
                            ComplexCategoryId = cat.Id,
                            ComplexCategoryCode = cat.Code,
                            ComplexCategoryName = cat.Name,
                            Quantity = dayd.Quantity,
                            Price = dayd.Price,
                            Date = daydate,
                            Confirmed = uday.IsConfirmed,
                            Total = uday.Total,
                            TotalWithoutDiscount = uday.TotalWtithoutDiscount,
                            Discount = uday.Discount,
                            Enabled = dayd.Date == daydate,  /*dayd != null*/
                            ComplexDishes = from d in _context.Dish.WhereCompany(companyid)
                                                //join dc in _context.DishComplex.WhereCompany(companyid) on d.Id equals dc.DishId
                                            join udd in _context.UserDayDish.WhereCompany(companyid).Where(i => i.Date == daydate && i.UserId == userId && i.ComplexId == comp.Id) on d.Id equals udd.DishId
                                            join dc in _context.DishComplex.WhereCompany(companyid).Where(d => d.ComplexId == comp.Id) on d.Id equals dc.DishId into leftdk
                                            from subdk in leftdk.DefaultIfEmpty()
                                            where udd.ComplexId == comp.Id
                                            orderby subdk.DishCourse
                                            select new UserDayComplexDishViewModel()
                                            {

                                                DishId = d.Id,
                                                DishName = d.Name,
                                                DishReadyWeight = d.ReadyWeight,
                                                PictureId = d.PictureId,
                                                DishCourse = subdk.DishCourse,
                                                DishQuantity = udd.Quantity,

                                                DishDescription = d.Description,
                                                DishIngredients = ""/* string.Join(",", from di in _context.DishIngredients.WhereCompany(companyid).Where(t => t.DishId == d.Id)
                                                                                   join ingr in _context.Ingredients on di.IngredientId equals ingr.Id
                                                                                   select ingr.Name)*/
                                            }
                        };
            var query1 = query.ToList();
            var confirmed1 = confirmed.ToList();
            query1.ForEach(d => d.Confirmed = confirmed1.FirstOrDefault().IsConfirmed);
            return query;
        }
        public IQueryable<UserDayDishViewModel> OrderedDishesDay(DateTime daydate, string userId, int companyid)
        {
            
            var query = from dish in _context.Dish
                        join uday in _context.UserDay.Where(ud => ud.HotelId == companyid & ud.Date == daydate && ud.UserId == userId) on dish.HotelId equals uday.HotelId
                        join dd in (from usubday in _context.UserDayDish where usubday.UserId == userId && usubday.Date == daydate && usubday.HotelId == companyid && usubday.IsComplex==false select usubday) on dish.Id equals dd.DishId into proto
                        from dayd in proto.DefaultIfEmpty()
                        join cat in _context.Categories.WhereCompany(companyid) on dayd.CategoriesId equals cat.Id
                        where dayd.Quantity > 0
                        orderby cat.Code
                        select new UserDayDishViewModel()
                        {
                            DishId = dish.Id,
                            DishName = dish.Name,
                            DishDescription= dish.Description,
                            MeasureUnit = dish.MeasureUnit,
                            CategoryId = cat.Id,
                            //DishKindId = dayd.DishKindId,
                            //OrderBaseWeight = dayd.Base,
                            CategoryName= cat.Name,
                            CategoryCode = cat.Code,
                            //IsWeight = dish.IsWeight,
                            Quantity = dayd.Quantity,
                            Price = dayd.Price,
                            Date = daydate,
                            PictureId = dish.PictureId,
                            Confirmed = uday.IsConfirmed,
                            Total = uday.Total,
                            //TotalWithoutDiscount = uday.TotalWtithoutDiscount,
                            //Discount = uday.Discount,
                            Enabled = dayd.Date == daydate,  /*dayd != null*/
                           
                        };
         
            return query;
        }
        public IEnumerable<UserOrderedDay> UserOrderedDay(DateTime daydate, string userId, int companyid)
        {
            var complexes = OrderedComplexDay(daydate, userId, companyid);
            var dishes = OrderedDishesDay(daydate, userId, companyid);
            var categories = complexes.Select(ord => ord.ComplexCategoryId)
                .Union(dishes.Select(ord => ord.CategoryId)).ToList();
            List<UserOrderedDay> res = new List<UserOrderedDay>();
            
            categories.ForEach(cat =>
            {
                UserOrderedDay day = new UserOrderedDay();
                day.Date = daydate;
                day.CategoryId = cat;
                if(complexes.Where(c => c.ComplexCategoryId == cat).ToList().Count() == 0)
                {
                    day.CategoryName = dishes.Where(com => com.CategoryId == cat).ToList().First().CategoryName;
                    day.CategoryCode = dishes.Where(com => com.CategoryId == cat).ToList().First().CategoryCode;
                }
                else
                {
                    //var categ = complexes.Where(com => com.ComplexCategoryId == cat).ToList().First();
                    day.CategoryName = complexes.Where(com => com.ComplexCategoryId == cat).ToList().First().ComplexCategoryName;
                    day.CategoryCode = complexes.Where(com => com.ComplexCategoryId == cat).ToList().First().ComplexCategoryCode;
                }
                day.UserDayComplex = complexes.Where(c => c.ComplexCategoryId == cat).ToList();
                day.UserDayDish = dishes.Where(c => c.CategoryId == cat).ToList();
                if (complexes.ToList().Count() != 0)
                {
                    day.Total = complexes.ToList().First().Total;
                    day.TotalWithoutDiscount = complexes.ToList().First().TotalWithoutDiscount;
                    day.DiscountSum = complexes.ToList().First().Discount;
                }
                else
                {
                    day.Total = dishes.ToList().First().Total;
                    day.TotalWithoutDiscount = dishes.ToList().First().Total;
                    day.DiscountSum = 0;
                }
                res.Add(day);
            });
            return res;
        }
        //public IEnumerable<SelectListItem> DishesKind(DateTime dateFrom, DateTime dateTo, int companyid)
        //{
        //    var query = (from udc in _context.DayComplex.WhereCompany(companyid).Where(u => u.Date >= dateFrom && u.Date <= dateTo)
        //                 join com in _context.Complex on udc.ComplexId equals com.Id
        //                 //join dc in _context.DishesKind on com.DishKindId equals dc.Id
        //                 select new
        //                 {
        //                     Code = dc.Code,
        //                     Value = dc.Id.ToString(),
        //                     Text = dc.Name
        //                 }).Distinct().OrderBy(dc => dc.Code).ToList();
        //    var query1 = (from dc in _context.DishesKind 
        //                  join dd in _context.DayDish.WhereCompany(companyid).Where(u => u.Date >= dateFrom && u.Date <= dateTo) on dc.Id equals dd.DishKindId
        //                  select new
        //                 {
        //                     Code = dc.Code,
        //                     Value = dc.Id.ToString(),
        //                     Text = dc.Name
        //                 }).Distinct().OrderBy(dc => dc.Code).ToList();
        //    var queryUnion = query.Union(query1).OrderBy(dc => dc.Code).ToList();
        //    List<SelectListItem> res = new List<SelectListItem>();
        //    queryUnion.ForEach(dc =>
        //    {
        //        res.Add(new SelectListItem() { Value = dc.Value, Text = dc.Text });
        //    });

        //    return res;
        //}
        //public string GetUserType(string userId)
        //{
        //    var user = _context.HotelUser.Where(u => u.Id == userId).FirstOrDefault();
        //    string type = "Child";
        //    if (user == null)
        //    {
        //        return type;
        //    }
        //    type = user.UserTypeEn.ToString();
        //    return type;
        //}
        //public bool CanUserOrder(string userId, int companyId, string userType,decimal amount)
        //{
        //    var getLimit = _context.CompanyOption.WhereCompany(companyId).
        //           FirstOrDefault(opt => opt.Name == userType).Value;
        //    decimal limit = decimal.Parse(getLimit);
        //    var userFin = _context.UserFinances.SingleOrDefault(ud => ud.Id == userId);
        //    return limit <userFin.Balance-userFin.TotalPreOrderedAmount-amount;
        //}
        //public async Task<decimal> UserDayLimit(string userId, int companyId, string userType, DateTime date)
        //{
        //    var getLimit =  _context.CompanyOption.WhereCompany(companyId).
        //           FirstOrDefault(opt => opt.Name == userType).Value;
        //    decimal limit = decimal.Parse(getLimit);
        //    var userFin = await  _context.UserFinances.SingleOrDefaultAsync(ud => ud.Id == userId);
        //    var userDayDishes = await _context.UserDayDish.WhereCompany(companyId).Where(ud => ud.UserId == userId && ud.IsComplex == false && ud.Date == date).ToListAsync();
        //    decimal udSum = 0;
        //    userDayDishes.ForEach(ud =>
        //    {
        //        udSum += ud.Quantity * ud.Price;
        //    });
        //    decimal limit1 = userFin.Balance - userFin.TotalPreOrderedAmount - limit;
        //    decimal limit2 = userFin.DayLimit - udSum;
        //    return Math.Min(limit1,limit2);
        //}
        //private async Task<bool> MakeOrderPaymentAsync(DateTime daydate, int companyId, string userId = null,string orderId=null)
        //{
        //    try
        //    {
        //        _logger.LogInformation("MakeUserOrderPayment {0},{1},{2}", daydate, companyId, userId);
        //        var res = await _context.Database.ExecuteSqlInterpolatedAsync($"exec MakeUserOrderPayment {daydate} , {companyId},{userId},{orderId}");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "MakeUserOrderPayment");
        //        return false;
        //    }
        //    return true;
        //}
    }
}
//to do make a separate context for async
/*
Func<UserDayDish, Task<bool>> saveday = async d =>  {

        var userDayDish = await _context.UserDayDish.FindAsync(this.User.GetUserId(), d.Date, d.DishId);
        if (userDayDish != null)
        {
            userDayDish.Quantity = d.Quantity;
            _context.Update(userDayDish);
        }
        else
        {
            d.UserId = _userManager.GetUserId(HttpContext.User);
            _context.Add(d);
        }

    return true;

};
*/
