using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacityStore.Models;
using System.Diagnostics;


namespace PharmacityStore.Controllers
{
    public class AccountController : Controller
    {

        private PharmacityContext _ctx;
        

        public AccountController( PharmacityContext ctx)
        {
            _ctx = ctx;
     
           
          
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
       /* private void SetAuthenticationCookie(int userId)
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


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        //Dang nhap
        [HttpPost]
        public IActionResult Login(User user)
        {
            User existingUser = _userDAO.GetUserByEmail(user.Email);

            if (existingUser == null || existingUser.Password != user.Password)
            {
                ModelState.AddModelError("LoginError", "Invalid email or password");
                return View();
            }

            if (existingUser.IsAdmin == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                SetAuthenticationCookie(existingUser.UserId);
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public IActionResult Logout()
        {

            HttpContext.SignOutAsync(); // Đăng xuất người dùng

            // Chuyển hướng đến trang chủ 
            return RedirectToAction("Index");
        }*/

   

      
      
        



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