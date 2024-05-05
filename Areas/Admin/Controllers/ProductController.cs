using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PharmacityStore.Models;
using PharmacityStore.Repository;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace PharmacityStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private ICategoryRepository _categoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(ICategoryRepository categoryRepository, 
            IProductRepository productRepository,IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _httpContextAccessor= httpContextAccessor;
    }

        
        public IActionResult ViewAllProduct()
        {
            
            var q1 = from c in _categoryRepository.GetAll()
                     select new SelectListItem()
                     {
                         Text = c.CateName,
                         Value = c.CateId.ToString(),
                     };

            ViewBag.CateId = q1.ToList();
            //1 get data
            List<Product> lst = _productRepository.GetAll();
            //2 data to view
            return View("ViewAllProduct", lst);
        }
        [HttpGet]
        public IActionResult CreateProduct()
        {
            
            var q1 = from c in _categoryRepository.GetAll()
                     select new SelectListItem()
                     {
                         Text = c.CateName,
                         Value = c.CateId.ToString(),
                     };
           
            ViewBag.CateId = q1.ToList();
            return View("CreateProduct", new Product());
        }
        [HttpPost]
        public IActionResult SaveProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                bool isProductNameExist = _productRepository.checkname(product.ProductName);
                if (isProductNameExist)
                {
                    ModelState.AddModelError(string.Empty, "tên đã có");
                    return View("CreateProduct");
                }
                _productRepository.Create(product);

                return RedirectToAction("ViewAllProduct");

            }
            else
            {
                return View("CreateProduct");
            }
        }
        public IActionResult EditProduct(Int16 id)
        {
            
            var q1 = from c in _categoryRepository.GetAll()
                     select new SelectListItem()
                     {
                         Text = c.CateName,
                         Value = c.CateId.ToString(),
                     };

            ViewBag.CateId = q1.ToList();

            return View("EditProduct", _productRepository.FindById(id));
        }
        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
           
            if (ModelState.IsValid)
            {
                bool isProductNameExist = _productRepository.checkname(product.ProductName);
                if (isProductNameExist)
                {
                    ModelState.AddModelError(string.Empty, "tên đã có");
                    return View("EditProduct");


                }
                _productRepository.Update(product);

                return RedirectToAction("ViewAllProduct");

            }
            else
            {
                return View("EditProduct");
            }
        }
        public IActionResult Delete(Int16 productId)
        {
            
            _productRepository.Delete(productId);
            return RedirectToAction("ViewAllProduct");
        }
        public IActionResult DeleteProduct(Int16 id)
        {
            return View("DeleteProduct", _productRepository.FindById(id));
        }

    }
}
