
using Microsoft.AspNetCore.Mvc;
using PharmacityStore.Models;
using PharmacityStore.Repository;
using System.Diagnostics;
namespace PharmacityStore.Controllers
{
    public class ProductController : Controller
    {
        private PharmacityContext _ctx;
        private readonly IProductRepository _productRepository;

        public ProductController( IProductRepository productRepository, PharmacityContext ctx)
        {
            _ctx = ctx;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            //1.Call dao
            List<Product> ds = _productRepository.SortProductsByPriceDescending();
            //2.passing data
            return View(ds);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
