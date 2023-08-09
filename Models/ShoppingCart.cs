using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication8.Helpers;


namespace WebApplication8.Models
{
	public class ShoppingCart 
	{
		private readonly liquorstoredbContext _context;
		private IHttpContextAccessor _httpContextAccessor;
        private static IHttpContextAccessor _StatichttpContextAccessor;
        public ShoppingCart(liquorstoredbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
        }
        public static void SetHttpContextAccessor(IHttpContextAccessor accessor)
        {
            _StatichttpContextAccessor = accessor;
        }

        public string Id { get; set; }
		public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; }

		public static ShoppingCart GetCart(IServiceProvider services)
		{
			ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
			var context = services.GetService<liquorstoredbContext>();
			string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

			session.SetString("CartId", cartId);
			return new ShoppingCart(context,null) { Id = cartId };
		}

		public bool AddToCart(int id, int Quantity)
		{
			bool IsFlag = false;
			if (SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart") == null)
			{
                List<ShoppingCartItem> cart = new List<ShoppingCartItem>();
                cart.Add(new ShoppingCartItem { product = _context.Products.Find(id), Quantity = 1,Id=id });
                SessionHelper.SetObjectAsJson(_httpContextAccessor.HttpContext.Session, "cart", cart);
				IsFlag = true;

			}
            else
            {
                List<ShoppingCartItem> cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new ShoppingCartItem { product = _context.Products.Find(id), Quantity = 1, Id = id });
                }
                SessionHelper.SetObjectAsJson(_httpContextAccessor.HttpContext.Session, "cart", cart);
				IsFlag = true;
			}
			return IsFlag;
		}
		private int isExist(int id)
		{
			List<ShoppingCartItem> cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart");
			for (int i = 0; i < cart.Count; i++)
			{
				if (cart[i].product.ProductId.Equals(id))
				{
					return i;
				}
			}
			return -1;
		}
        public int RemoveFromCart(Product product)
        {
            var cart= SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart");
            var shoppingCartItem = cart.SingleOrDefault(s => s.product.ProductId == product.ProductId);
            int localQuantity = 0;
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity--;
                    localQuantity = shoppingCartItem.Quantity;
                }
                else
                {
                    cart.Remove(shoppingCartItem);
                }
            }
            SessionHelper.SetObjectAsJson(_httpContextAccessor.HttpContext.Session, "cart", cart);
            return localQuantity;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            List<ShoppingCartItem> cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart");
            return cart;
        }
        public static int GetShoppingCartCount()
        {
            List<ShoppingCartItem> cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_StatichttpContextAccessor.HttpContext.Session, "cart");
            if (cart!=null)
            {
                return cart.Count;
            }
            return 0;
        }
        public void ClearCart()
        {
            var cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart");
            cart.Clear();
            SessionHelper.SetObjectAsJson(_httpContextAccessor.HttpContext.Session, "cart", cart);
        }

        public decimal GetShoppingCartTotal()
        {
            var cart = SessionHelper.GetObjectFromJson<List<ShoppingCartItem>>(_httpContextAccessor.HttpContext.Session, "cart");
            if (cart != null)
            {
                return Convert.ToDecimal(cart.Select(c => c.product.Price * c.Quantity).Sum());
            }
            return 0;
            
        }

    }
}
