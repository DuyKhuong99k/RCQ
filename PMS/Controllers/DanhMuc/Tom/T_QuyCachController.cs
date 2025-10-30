using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_QuyCachController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "QuyCachView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "QuyCachView";
            return View("~/Views/DanhMuc/Tom/QuyCachView.cshtml");
        }
    }
}
