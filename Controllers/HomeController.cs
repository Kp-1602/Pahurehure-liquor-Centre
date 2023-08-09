using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly liquorstoredbContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        public HomeController(liquorstoredbContext context, IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            var cartitems = shoppingCart.GetShoppingCartItems();
            if (cartitems != null)
                ViewBag.CartCount = cartitems.Count;
            else ViewBag.CartCount = 0;
            List<Product> products = _context.Products.Include(p => p.Department.SubDepartment).OrderByDescending(x => x.ProductId).Take(16).ToList();
            return View(products);
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
