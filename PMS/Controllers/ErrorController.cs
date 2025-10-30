using Microsoft.AspNetCore.Mvc;

namespace PMS.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound ()
        {
            return View();
        }
        public IActionResult Unauthorized()
        {
            return View();
        }
        public IActionResult StopAttack()
        {
            return View();
        }
    }
}
