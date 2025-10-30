using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_ChiTietVoXoController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "T_ChiTietVoXoView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "T_ChiTietVoXoView";
            return View("~/Views/DanhMuc/Tom/ChiTietVoXoView.cshtml");
        }
    }
}
