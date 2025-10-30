using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    [Authorize]
    public class T_TyLeHoaChatTheoNhomController : Controller
    {
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "TyLeHoaChatTheoNhomView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "TyLeHoaChatTheoNhomView";
            return View("~/Views/DanhMuc/Tom/TyLeHoaChatTheoNhomView.cshtml");
        }
    }
}
