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
using PMSHub.Middlewares;
using Services;
using ViewModels.Repos.Hubs.IServices;

internal static class LogoutHttpChecks
{
    public static async Task Run(string root, Action<bool, string> check)
    {
        var production = File.ReadAllText(Path.Combine(root, "PMSHub/Program.cs"));
        check(production.Contains("builder.Services.AddControllersWithViews().AddNewtonsoftJson"),
            "Production đăng ký MVC ViewFeatures cần cho ValidateAntiForgeryToken");
        check(production.IndexOf("app.UseMiddleware<LegacyLoginRedirectMiddleware>();", StringComparison.Ordinal)
            < production.IndexOf("app.UseSession();", StringComparison.Ordinal)
            && production.Contains("app.UseMiddleware<LegacyLoginRedirectMiddleware>();"),
            "Chặn URL cũ trước middleware session");
        check(!typeof(LoginService).GetMethods().Any(x => x.Name == "Login")
            && !typeof(ILoginService).GetMethods().Any(x => x.Name == "Login"),
            "Không còn API tạo phiên chỉ bằng username/token chưa xác thực");
        var legacyPage = File.ReadAllText(Path.Combine(root, "PMSHub/Components/Pages/Login.razor"));
        check(!legacyPage.Contains("loginService") && legacyPage.Contains("/api/loginlocal/"),
            "Điều hướng Blazor nội bộ qua trang cũ cũng không đăng nhập");

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
        app.UseMiddleware<LegacyLoginRedirectMiddleware>();
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
            foreach(var path in new[] {"/api/login/test-user/not-a-token", "/API/LOGIN/test-user/anything?returnUrl=https://example.invalid", "/api/login"}) {
                using var legacy = await client.GetAsync(path);
                check(legacy.StatusCode == HttpStatusCode.Redirect
                    && legacy.Headers.Location?.OriginalString == "/api/loginlocal/",
                    "HTTP: URL token cũ chỉ về đăng nhập, không nhận returnUrl bên ngoài");
                check(!legacy.Headers.Contains("Set-Cookie") && legacy.Headers.CacheControl?.NoStore == true,
                    "HTTP: URL cũ không tạo cookie phiên và không cache phản hồi");
            }
            check(await client.GetStringAsync("/test/state") == "||", "HTTP: URL cũ không tạo danh tính hoặc xưởng");
            using var loginLocal = await client.GetAsync("/api/loginlocal/");
            check(loginLocal.StatusCode == HttpStatusCode.NotFound,
                "HTTP: middleware không chặn /api/loginlocal/ (host test không map trang Blazor)");
            var token = await client.GetStringAsync("/test/seed");
            check(await client.GetStringAsync("/test/state") == "1|test-user|1", "HTTP: tạo và đọc phiên thật qua cookie");
            using var legacyPost = await client.PostAsync("/api/login/other-user/fake", new StringContent(""));
            check(legacyPost.StatusCode == HttpStatusCode.Redirect
                && await client.GetStringAsync("/test/state") == "1|test-user|1",
                "HTTP: URL cũ không thay danh tính/xưởng của phiên đã đăng nhập");
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
