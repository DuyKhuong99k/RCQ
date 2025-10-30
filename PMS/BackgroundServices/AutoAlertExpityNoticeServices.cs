using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PMS.Controllers;
using PMS.Hubs;

namespace PMS.BackgroundServices
{
    public class AutoAlertExpityNoticeServices : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public AutoAlertExpityNoticeServices(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime startDate = new DateTime(2025, 8, 17);
            //DateTime startDate = new DateTime(2025, 8, 15, 11,55,0);
            while (!stoppingToken.IsCancellationRequested)
            {
                string messageVm = AppViewModels.AppViewModel.Instance.MessageExpityNotece;
                DateTime now = DateTime.Now;

                if (!string.IsNullOrEmpty(messageVm) && now >= startDate)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<ProgressHub>>();

                    var message = new
                    {
                        Title = "Thông Báo",
                        Content = messageVm,
                        Time = now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    await hub.Clients.All.SendAsync("ReceiveMessage", message, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // Test 1s
            }
        }
    }
}
