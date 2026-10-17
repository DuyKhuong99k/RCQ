namespace PMSHub.Middlewares;

/// <summary>URL đăng nhập cũ không còn là chứng thực danh tính.</summary>
public sealed class LegacyLoginRedirectMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/login", StringComparison.OrdinalIgnoreCase))
        {
            // Không đọc username/token, không sửa phiên, không đưa tham số cũ vào redirect.
            context.Response.Headers.CacheControl = "no-store";
            context.Response.Redirect(context.Request.PathBase + "/api/loginlocal/");
            return Task.CompletedTask;
        }
        return next(context);
    }
}
