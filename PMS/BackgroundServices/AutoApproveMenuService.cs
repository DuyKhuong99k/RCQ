using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PMS.Controllers.NhaAn.DanhMucNhaAn;
using PMS.Hubs;

public class AutoApproveMenuService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AutoApproveMenuService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRunTime = DateTime.Today.AddHours(12); // 12h trưa hôm nay
            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1); // nếu đã quá 12h thì đặt sang ngày mai
            }

            var delay = nextRunTime - now;

            // Chờ đến 12h trưa
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                break;

            using var scope = _scopeFactory.CreateScope();
            var controller = scope.ServiceProvider.GetRequiredService<HQ_ThucDonController>();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await controller.AutoApproveMenu();

            var hub = scope.ServiceProvider.GetService<IHubContext<ProgressHub>>();
            if (result is JsonResult jsonResult)
            {
                await hub.Clients.All.SendAsync("ReceiveMessage", jsonResult.Value);
            }
        }

    }
}
