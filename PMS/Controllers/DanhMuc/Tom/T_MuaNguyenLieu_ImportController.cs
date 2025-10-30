using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_MuaNguyenLieu_ImportController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "T_MuaNguyenLieu_ImportView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "T_MuaNguyenLieu_ImportView";
            return View("~/Views/DanhMuc/Tom/MuaNguyenLieu_ImportView.cshtml");
        }
    }
}
