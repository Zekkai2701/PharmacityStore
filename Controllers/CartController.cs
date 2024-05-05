using Microsoft.AspNetCore.Mvc;

namespace PharmacityStore.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
