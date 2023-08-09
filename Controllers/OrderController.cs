using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class OrderController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private static UserManager<IdentityUser> _userManager;
        private readonly liquorstoredbContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        public OrderController(liquorstoredbContext context, ShoppingCart shoppingCart, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _shoppingCart = shoppingCart;
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Checkout()
        {
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            var items = shoppingCart.GetShoppingCartItems();
            shoppingCart.ShoppingCartItems = items;
            amount = (long)(items.Sum(item => item.Quantity * item.product.Price) * 100);
            ViewBag.PaymentAmount = (items.Sum(item => item.Quantity * item.product.Price)); ////Converted into Cents
            if (items.Count() == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some items first");
                return RedirectToAction("Index", "Home");
            }
            ShoppingCartIndexModel model = new ShoppingCartIndexModel();
            model.ShoppingCartTotal = Convert.ToDecimal(items.Sum(item => item.Quantity * item.product.Price));
            model.ShoppingCart = items;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(ShoppingCartIndexModel model)
        {
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            var items = shoppingCart.GetShoppingCartItems();
            var order = new Order();
            var orderDetailLst = new List<OrderDetail>();
            _shoppingCart.ShoppingCartItems = items;

            if (items.Count() == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some items first");
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);

                _context.Customers.Add(model.customer);
                _context.SaveChanges();


                Random rnd2 = new Random();
                order.OrderNo = rnd2.Next(1111111, 9999999);
                order.OrderTotal = items.Sum(item => item.Quantity * item.product.Price);
                order.OrderDate = DateTime.Now;
                order.PaymentType = "Card";
                order.IsPaid = "Yes";
                order.CustomerId = model.customer.CustomerId;
                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var item in items)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Price = item.product.Price;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.Subtotal = item.Quantity * item.product.Price;
                    orderDetailLst.Add(orderDetail);
                }
                _context.AddRange(orderDetailLst);
                _context.SaveChanges();

                shoppingCart.ClearCart();
                return RedirectToAction("Processing");
            }
            return View(model);
        }
        private static long? amount { get; set; }
        public IActionResult Processing(ShoppingCartIndexModel model)
        {
            Dictionary<string, string> Metadata = new Dictionary<string, string>();
            Metadata.Add("Product", "ABC");
            Metadata.Add("Quantity", "1");
            var options = new ChargeCreateOptions
            {
                Amount = amount,//(long)amount,
                Currency = "NZD",
                Description = "Online Shopping",
                Source = model.card.stripeToken,
                ReceiptEmail = model.customer.CustomerEmail,
                Metadata = Metadata
            };
            var Addoptions = new AddressOptions
            {
                Line1 = model.customer.Address,
                PostalCode=model.customer.PostCode.ToString(),
                City=model.customer.City
            };
            var cusoptions = new CustomerCreateOptions
            {
                Name = model.customer.FirstName + ' ' + model.customer.FirstName,
                Address = Addoptions,
                Email = model.customer.CustomerEmail
            };
            var service = new ChargeService();
            var customer = new CustomerService();
            Charge charge = service.Create(options);
            Stripe.Customer cus = customer.Create(cusoptions);
            string TransactionId = charge.BalanceTransactionId;
            /////////////
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            var items = shoppingCart.GetShoppingCartItems();
            var order = new Order();
            var orderDetailLst = new List<OrderDetail>();
            _shoppingCart.ShoppingCartItems = items;

            if (items.Count() == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, add some items first");
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Customers.Add(model.customer);
                _context.SaveChanges();

                Random rnd2 = new Random();
                order.OrderNo = rnd2.Next(1111111, 9999999);
                order.OrderTotal = items.Sum(item => item.Quantity * item.product.Price);
                order.OrderDate = DateTime.Now;
                order.PaymentType = "Card";
                order.IsPaid = "Yes";
                order.CustomerId = model.customer.CustomerId;
                order.PickUpTime = model.PickTime;
                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var item in items)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductId = item.product.ProductId;
                    orderDetail.Price = item.product.Price;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.OrderId = order.OrderId;
                    orderDetail.Subtotal = item.Quantity * item.product.Price;
                    orderDetailLst.Add(orderDetail);
                }
                _context.AddRange(orderDetailLst);
                _context.SaveChanges();

                shoppingCart.ClearCart();
                return RedirectToAction("Complete", new { id = model.PickTime.Value.ToString() });
            }
            /////////////
            return View();
        }
        public IActionResult Complete(string id)
        {
            string PickTime = id.Replace("%2F", "/");
            ViewBag.CheckoutCompleteMessage = "Thanks for your order and your pickup time is @"+ PickTime + ". The digital receipt has been sent to you if you enetered your email.";
            return View();
        }
    }
}