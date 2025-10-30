using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_TrangThaiNguyenLieuController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TrangThaiNguyenLieuView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TrangThaiNguyenLieuView";
            return View("~/Views/DanhMuc/Tom/TrangThaiNguyenLieuView.cshtml");
        }
    }
}
