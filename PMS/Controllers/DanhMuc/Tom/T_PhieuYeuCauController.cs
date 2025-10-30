using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_PhieuYeuCauController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "PhieuYeuCauView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "PhieuYeuCauView";
            return View("~/Views/DanhMuc/Tom/PhieuYeuCauView.cshtml");
        }
    }
}
