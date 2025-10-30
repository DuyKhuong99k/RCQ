using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_PhuGiaController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhuGiaView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "PhieuYeuCauView";
            return View("~/Views/DanhMuc/Tom/PhuGiaView.cshtml");
        }
    }
}
