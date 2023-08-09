using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication8.Models;
using X.PagedList;

namespace WebApplication8.Controllers
{
    public class ProductsController : Controller
    {
        private readonly liquorstoredbContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        public ProductsController(liquorstoredbContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: Products
        public IActionResult Index(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT * FROM Products WHERE Name LIKE @p0 "; // Contains An     
            string wrapString = "%" + searchString + "%";
            switch (sortOrder)
            {
                case "name_desc":
                    sql = sql + "ORDER BY Name DESC";
                    break;
                case "name_asc":
                    sql = sql + "ORDER BY Name ASC";
                    break;
            }

            List<Product> products = _context.Products.FromSqlRaw(sql, wrapString).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        [HttpGet]
        public IActionResult Add(int id, int? qty = 1)
        {
            var product = _context.Products.Find(id);
            bool isValidAmount = false;
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            if (product != null)
            {
                isValidAmount = shoppingCart.AddToCart(id, qty.Value);
            }
            return RedirectToAction("Index", "ShoppingCart");
        }
        public IActionResult UpdateCart(int id, int? qty=1)
        {
            var product = _context.Products.Find(id);
            bool isValidAmount = false;
            ShoppingCart shoppingCart = new ShoppingCart(new liquorstoredbContext(), _httpContextAccessor);
            if (product != null)
            {
                isValidAmount = shoppingCart.AddToCart(id, qty.Value);
            }
            return RedirectToAction("Index", "ShoppingCart");
        }
        public string IndexAJAX(string searchString)
        {
            string sql = "SELECT * FROM Products WHERE Name LIKE @p0"; // Contains An     
            string wrapString = "%" + searchString + "%";
            List<Product> movies = _context.Products.FromSqlRaw(sql, wrapString).ToList();
            string jason = JsonConvert.SerializeObject(movies);
            return jason;
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Price,Description,Image,DepartmentId,Stock")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", product.DepartmentId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", product.DepartmentId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Price,Description,Image,DepartmentId,Stock")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", product.DepartmentId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public IActionResult Wine(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 2; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        public IActionResult Spirit(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 1; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        public IActionResult Port(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 4; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        public IActionResult Lowalcohol(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 7; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        public IActionResult Beer(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 5; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        public IActionResult Premix(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 6; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
        public IActionResult Craft(string searchString, int? page, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var pageNumber = page ?? 1; // if no page was specified in the querystring, deafult 
                                        //to the first page 

            string sql = "SELECT ProductId,Products.Name, Products.Price, Products.Description,Products.DepartmentId ,Products.Image,Products.Stock FROM Products INNER JOIN Department ON Products.DepartmentId = Department.DepartmentId WHERE Sub_DepartmentId = 8; ";

            List<Product> products = _context.Products.FromSqlRaw(sql).ToList();
            return View(products.ToPagedList(pageNumber, 12));

        }
    }
}
