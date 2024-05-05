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

    public class HomeController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private IUserRepository _userRepository;

        public HomeController(ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IHttpContextAccessor httpContextAccessor,
            
            IUserRepository userRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
          
            _userRepository = userRepository;
        }
        private void SetAuthenticationCookie(int userId)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
                ).Wait();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Dang nhap
        [HttpPost]
        public IActionResult Login(User user)
        {
            User existingUser = _userRepository.GetUserByEmail(user.Email);

            if (existingUser == null || existingUser.Password != user.Password)
            {
                ModelState.AddModelError("LoginError", "Invalid email or password");
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, existingUser.UserId.ToString())
    };

            if (existingUser.IsAdmin == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))).Wait();
                return RedirectToAction("Index", "Admin");
            }

            HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))).Wait();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult ViewAllCategories()
        {
           
            //1 get data
            List<Category> lst = _categoryRepository.GetAll();
            //2 data to view
            return View("ViewAllCategories", lst);
        }
        
        [HttpGet]
        
        public IActionResult CreateCategory()
        {
           
            return View("CreateCategory", new Category());
        }
        
        [HttpGet]
        public IActionResult CreateToy()
        {
           
            var q1 = from c in _categoryRepository.GetAll()
                     select new SelectListItem()
                     {
                         Text = c.CateName,
                         Value = c.CateId.ToString()
                     };
            var q2 = from c in _productRepository.GetAll()
                     select new SelectListItem()
                     {
                         Text = c.ProductName,
                         Value = c.ProductId.ToString()
                     };
            ViewBag.CateId = q1.ToList();
            ViewBag.ProductId = q2.ToList();
            return View("CreateToy", new Product());
        }
      
        [HttpPost]
        public IActionResult SaveCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                bool isCategoryNameExist = _categoryRepository.checkname(category.CateName);
                if (isCategoryNameExist)
                {
                    ModelState.AddModelError(string.Empty, "tên đã có");
                    return View("CreateCategory");
                }
                _categoryRepository.Create(category);

                return RedirectToAction("ViewAllCategories");

            }
            else
            {
                return View("CreateCategory");
            }
        }
      
        public IActionResult EditCategory(Int16 id)
        {
            
            return View("EditCategory", _categoryRepository.FindById(id));
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                bool isCategoryNameExist = _categoryRepository.checkname(category.CateName);
                if (isCategoryNameExist)
                {
                    ModelState.AddModelError(string.Empty, "tên đã có");
                    return View("EditCategory");


                }
                _categoryRepository.Update(category);

                return RedirectToAction("ViewAllCategories");

            }
            else
            {
                return View("EditCategory");
            }
        }
        public IActionResult Delete(Int16 cateId)
        {
            
            _categoryRepository.Delete(cateId);
            return RedirectToAction("ViewAllCategories");
        }
        public IActionResult DeleteCategory(Int16 id)
        {
            return View("DeleteCategory", _categoryRepository.FindById(id));
        }

    }

}

