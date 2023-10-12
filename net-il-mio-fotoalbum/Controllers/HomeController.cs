using net_il_mio_fotoalbum.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace net_il_mio_fotoalbum_static.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}