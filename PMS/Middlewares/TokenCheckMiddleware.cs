using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PMS.Middlewares
{
    public class TokenCheckMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            if (!HasValidAuthentication(context))
            {
                if (!context.Request.Path.StartsWithSegments("/Authentication/Login"))
                {
                    context.Response.Redirect("/Authentication/Login");
                    return;
                }
            }

            await _next(context);
        }

        private bool HasValidAuthentication(HttpContext context)
        {
            // Kiểm tra xem có phiên đăng nhập (session) nào không
            var loggedIn = context.Session.GetString("JWTToken");
            return !string.IsNullOrEmpty(loggedIn) && Convert.ToBoolean(loggedIn);
        }
    }
}
