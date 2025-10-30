using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_CongDoanTheoThanhPhamController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "CongDoanTheoThanhPhamView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "T_CongDoanTheoThanhPham";
            return View("~/Views/DanhMuc/Tom/CongDoanTheoThanhPhamView.cshtml");
        }
    }
}
