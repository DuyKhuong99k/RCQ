using Microsoft.AspNetCore.Mvc;

namespace PMSHub.Controllers;

// Kết thúc session trong HTTP request để SessionMiddleware lưu thay đổi,
// không sửa HttpContext đã cũ của circuit Blazor.
[Route("api/session")]
public class SessionController : Controller
{
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.Session.CommitAsync(HttpContext.RequestAborted);
        // Không RemoveUserArea: tài khoản có thể còn đăng nhập ở máy khác.
        return LocalRedirect("~/api/loginlocal/");
    }
}
