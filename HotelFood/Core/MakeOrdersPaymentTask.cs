using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HotelFood.Data;
using HotelFood.Models;

namespace HotelFood.Core
{

    public class MakeOrdersPaymentTask : IScheduledTask
    {


        private readonly ILogger<HotelUser> _logger;
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        public MakeOrdersPaymentTask(ILogger<HotelUser> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {

            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }
        public bool IsRunning { get; private set; }
#if DEBUG
        public string Schedule => "*/5 * * * *"; //every 5 minutes
#else
        public string Schedule => "0 23 * * *"; //every day  at 23:00
#endif
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start job to Write Off production day");
            IsRunning = true;
            try
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    AppDbContext context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                    if (context == null)
                    {
                        _logger.LogError("Can't  obtain DB context");
                        return;

                    }

                    foreach (var cmp in await context.Hotel.AsNoTracking().ToListAsync())
                    {
                        //IUserFinRepository finrepo = serviceScope.ServiceProvider.GetRequiredService<IUserFinRepository>();
                        //if (finrepo == null)
                        //{
                        //    _logger.LogError("Can't  obtain fin repository object");
                        //    return;

                        //}
                        //await finrepo.MakeOrderPaymentAsync(DateTime.Now, cmp.Id);

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Make order Payment error");
            }
            finally
            {
                IsRunning = false;
            }

        }
    }


}