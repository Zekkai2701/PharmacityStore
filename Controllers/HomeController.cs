using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacityStore.Models;
using System.Diagnostics;
using System.Security.Claims;
using PharmacityStore.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using PharmacityStore.Models;
using PharmacityStore.Repository;

namespace PharmacityStore.Controllers
{
    public class HomeController : Controller
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProductRepository _productRepository;
        private PharmacityContext _ctx;

        private readonly UserDAO _userDAO;
        private readonly CartDAO _cartDAO;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IInvoiceRepository invoiceRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            PharmacityContext ctx,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _ctx = ctx;

            _userDAO = new UserDAO(_ctx);
            _cartDAO = new CartDAO(_ctx);
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            _invoiceRepository = invoiceRepository;
        }

        public IActionResult Index()
        {
            //1. Call 
            List<Product> ds = _productRepository.GetAll();
            //2. Passing data
            return View(ds);
        }
        public IActionResult Detail(Int16 id)
        {

            //1. Call dao
            Product product = _productRepository.FindById(id);
            //2. Passing data
            return View(product);
        }
        public IActionResult Search(string productName)
        {
            List<Product> products = _productRepository.searchProductByName(productName);
            return View(products);
        }
        public IActionResult findProductByCategoryId(Int16 id)
        {
            List<Product> products = _productRepository.GetAllProductsById(id);
            return View(products);
        }
        /*private void SetAuthenticationCookie(int userId)
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
        }*/
        private bool IsValidEmail(string email)
        {
            // Sử dụng regular expression để kiểm tra định dạng email
            string pattern = @"^[A-Za-z0-9._%+-]+@gmail\.com$";
            return Regex.IsMatch(email, pattern);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Phương thức xử lý đăng ký
        [HttpPost]
        public IActionResult Register(User user)
        {
            // Kiểm tra xem người dùng đã tồn tại hay chưa
            if (_userDAO.GetUserByEmail(user.Email) != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View();
            }
            // Kiểm tra xem username và phone có để trống hay không
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                ModelState.AddModelError("UserName", "Username is required");
                return View();
            }

            if (string.IsNullOrWhiteSpace(user.Phone))
            {
                ModelState.AddModelError("Phone", "Phone is required");
                return View();
            }
            // Kiểm tra định dạng email
            if (!IsValidEmail(user.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format");
                return View();
            }


            // Kiểm tra và gán giá trị mặc định cho IsAdmin và Role
            user.IsAdmin = user.IsAdmin ?? 0;
            user.Role = user.Role ?? "khách";

            // Thêm người dùng mới vào cơ sở dữ liệu
            _userDAO.AddUser(user);

            // Chuyển hướng đến trang đăng nhập hoặc trang chính
            return RedirectToAction("Login");
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
            User existingUser = _userDAO.GetUserByEmail(user.Email);

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

        [HttpGet]
        public IActionResult Logout()
        {

            HttpContext.SignOutAsync(); // Đăng xuất người dùng

            // Chuyển hướng đến trang chủ 
            return RedirectToAction("Index");
        }

        // Lấy thông tin user ID từ cookie
        private int GetUserId()
        {
            if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {

                var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }

            return 0;
        }


        [HttpPost]
        public IActionResult AddToCart(Int16 productId, int quantity)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }

            // Lấy sản phẩm từ cơ sở dữ liệu
            Product product = _productRepository.FindById(productId);

            if (product == null)
            {
                return NotFound();
            }

            // Thêm sản phẩm vào giỏ hàng
            int userId = GetUserId();
            _cartDAO.AddToCart(product, quantity, userId);

            // Chuyển hướng đến trang giỏ hàng
            return RedirectToAction("Cart");
        }


        public IActionResult Cart()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }

            // Lấy danh sách sản phẩm trong giỏ hàng của người dùng hiện tại
            int userId = GetUserId();
            List<Cart> cartItems = _cartDAO.GetCartItemsByUserId(userId);

            // Passing data
            return View(cartItems);
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            // nhận userid hiện tại 
            int userId = GetUserId();

            // tìm đến cart item
            var cartItem = _ctx.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                //xóa cart item
                _ctx.Carts.Remove(cartItem);
                _ctx.SaveChanges();
            }

            return RedirectToAction("Cart");
        }



        public IActionResult PrintInvoice(Int16 invoiceId)
        {
            // nhận userid hiện tại 
            int userId = GetUserId();

            // tìm đến cart item
            List<Cart> cartItems = _ctx.Carts.Include(c => c.Product).Where(c => c.UserId == userId).ToList();

            // total amount
            decimal totalAmount = (decimal)cartItems.Sum(c => c.Price * c.Quantity);


            // Cập nhật trạng thái thanh toán cho các mục giỏ hàng
            foreach (var cartItem in cartItems)
            {
                cartItem.IsPaid = true;
            }

            Invoice invoice = new Invoice
            {
                CartId = cartItems.FirstOrDefault()?.CartId,
                UserId = userId,
                TotalAmount = totalAmount,
                InvoiceDate = DateTime.Now,
                Status = 0
            };

            // Save the invoice to the database
            _ctx.Invoices.Add(invoice);
            _ctx.SaveChanges();

            // Pass the invoice data to the view
            ViewData["CartItems"] = cartItems;
            ViewData["TotalAmount"] = totalAmount;

            return View(invoice);
        }

        public IActionResult Order()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }
            // Lấy User ID hiện tại
            int userId = GetUserId();

            // Lấy danh sách Invoice của User từ Repository
            List<Invoice> invoices = _invoiceRepository.GetInvoicesByUserId(userId);

            return View(invoices);
        }

        [HttpPost]

        public IActionResult DeleteCartByInvoiceStatus(int status)
        {
            // tìm đến status invoice của giỏ hàng này
            var cartsToDelete = _ctx.Carts.Include(c => c.Invoices)
            .Where(c => c.Invoices.Any(i => i.Status == status))
            .ToList();
            // xóa giỏ hàng
            _ctx.Carts.RemoveRange(cartsToDelete);

            // Save changes
            _ctx.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateInvoiceStatus(int invoiceId, int status)
        {
            var invoice = _ctx.Invoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
            if (invoice != null)
            {
                invoice.Status = status;
                _ctx.SaveChanges();
            }

            return RedirectToAction("Index");

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