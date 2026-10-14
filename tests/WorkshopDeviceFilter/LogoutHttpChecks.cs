using System.Net;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PMSHub.Controllers;

internal static class LogoutHttpChecks
{
    public static async Task Run(string root, Action<bool, string> check)
    {
        var production = File.ReadAllText(Path.Combine(root, "PMSHub/Program.cs"));
        check(production.Contains("builder.Services.AddControllersWithViews().AddNewtonsoftJson"),
            "Production đăng ký MVC ViewFeatures cần cho ValidateAntiForgeryToken");

        // Host cô lập: chỉ MVC/session/antiforgery; không chạy Program của Hub,
        // không SQL, hosted service, kết nối WSS hoặc lệnh tới thiết bị.
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
            ApplicationName = typeof(SessionController).Assembly.FullName,
            ContentRootPath = AppContext.BaseDirectory,
            EnvironmentName = "Development"
        });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();
        builder.Services.AddControllersWithViews().AddApplicationPart(typeof(SessionController).Assembly);
        await using var app = builder.Build();
        app.UseRouting();
        app.UseSession();
        app.UseAntiforgery();
        app.MapControllers();
        app.MapGet("/test/seed", async (HttpContext context, IAntiforgery antiforgery) => {
            context.Session.SetString("UserId", "1");
            context.Session.SetString("Username", "test-user");
            context.Session.SetString("XuongId", "1");
            var token = antiforgery.GetAndStoreTokens(context);
            await context.Response.WriteAsync(token.RequestToken!);
        });
        app.MapGet("/test/state", (HttpContext context) =>
            string.Join("|", new[] {"UserId", "Username", "XuongId"}.Select(key => context.Session.GetString(key) ?? "")));
        await app.StartAsync();
        try
        {
            var address = app.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()!.Addresses.Single();
            using var handler = new HttpClientHandler {AllowAutoRedirect = false, CookieContainer = new CookieContainer()};
            using var client = new HttpClient(handler) {BaseAddress = new Uri(address), Timeout = TimeSpan.FromSeconds(15)};
            var token = await client.GetStringAsync("/test/seed");
            check(await client.GetStringAsync("/test/state") == "1|test-user|1", "HTTP: tạo và đọc phiên thật qua cookie");
            using var noToken = await client.PostAsync("/api/session/logout", new FormUrlEncodedContent(new Dictionary<string, string>()));
            check(noToken.StatusCode == HttpStatusCode.BadRequest, "HTTP: thiếu antiforgery token trả 400, không lỗi 500 thiếu dịch vụ");
            check(await client.GetStringAsync("/test/state") == "1|test-user|1", "HTTP: yêu cầu không hợp lệ không xóa phiên");
            using var badToken = await client.PostAsync("/api/session/logout", new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = "invalid" }));
            check(badToken.StatusCode == HttpStatusCode.BadRequest, "HTTP: token giả bị chặn");
            using var logout = await client.PostAsync("/api/session/logout", new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token }));
            check(logout.StatusCode == HttpStatusCode.Redirect && logout.Headers.Location?.OriginalString == "/api/loginlocal/",
                "HTTP: form hợp lệ đi qua MVC filter, đăng xuất và redirect 302 đúng trang");
            check(await client.GetStringAsync("/test/state") == "||", "HTTP: request kế tiếp xác nhận phiên đã bị xóa thật");
            using var repeated = await client.PostAsync("/api/session/logout", new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token }));
            check(repeated.StatusCode == HttpStatusCode.Redirect, "HTTP: đăng xuất lặp với phiên đã hết vẫn hoạt động");
            using var get = await client.GetAsync("/api/session/logout");
            check(get.StatusCode == HttpStatusCode.MethodNotAllowed, "HTTP: không cho đăng xuất bằng GET");
        }
        finally { await app.StopAsync(); }
    }
}
