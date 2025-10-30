using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers.DanhMuc
{
    public class LoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
