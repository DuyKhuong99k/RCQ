using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc.Tom
{
    public class T_BieuMauGiao_ImportController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var rl = Middlewares.AuthenticationHelpers.CheckAut(HttpContext, "BieuMauGiao_ImportView");

            if (rl == false)
            {
                return Redirect(AppViewModels.AppViewModel.Instance.RedirectLoginUrl);
            }
            ViewBag.TitlePage = "BieuMauGiao_Import";
            return View("~/Views/DanhMuc/Tom/BieuMauGiao_ImportView.cshtml");
        }
    }
}
