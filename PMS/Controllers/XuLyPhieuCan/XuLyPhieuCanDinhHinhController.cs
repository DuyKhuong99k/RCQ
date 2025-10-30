using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.XuLyPhieuCan
{
    public class XuLyPhieuCanDinhHinhController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/XuLyPhieuCan/XuLyPhieuCanDinhHinhView.cshtml");
        }
    }
}
