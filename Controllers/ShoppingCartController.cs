using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly liquorstoredbContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        public ShoppingCartController(liquorstoredbContext context, ShoppingCart shoppingCart,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _shoppingCart = shoppingCart;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index(bool isValidAmount = true, string returnUrl = "/")
        {
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            var cartitems=shoppingCart.GetShoppingCartItems();
            var model = new ShoppingCartIndexModel
            {
                ShoppingCart = cartitems,
                ShoppingCartTotal = shoppingCart.GetShoppingCartTotal(),
                ReturnUrl = returnUrl
            };
            if (!isValidAmount)
            {
                ViewBag.InvalidAmountText = "*There were not enough items in stock to add*";
            }
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Add(int id, int? qty = 1, string returnUrl=null )
        {
            var product = _context.Products.Find(id);
            returnUrl = returnUrl.Replace("%2F", "/");
            bool isValidAmount = false;
            if (product != null)
            {
                isValidAmount = _shoppingCart.AddToCart(id, qty.Value);
            }

            return Index(isValidAmount, returnUrl);
        }

        public IActionResult Remove(int Id)
        {
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            var product = _context.Products.Find(Id);
            if (product != null)
            {
                shoppingCart.RemoveFromCart(product);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Back(string returnUrl="/")
        {
            return Redirect(returnUrl);
        }
    }
}