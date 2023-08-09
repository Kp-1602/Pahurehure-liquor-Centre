using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class ReportController : Controller
    {
        private readonly liquorstoredbContext _context;

        public ReportController(liquorstoredbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string sql = "SELECT[Order_Details].[OrderId],[Order].[Order_No],[Order].[Order_Date],[Order_Details].[ProductId],[Products].[Name],[Products].[Image],[Order_Details].[Quantity] ,[Order_Details].[Price],[Order_Details].[Subtotal], [Customer].[First_Name],[Customer].[Last_Name],[Customer].[Customer_Email],[Customer].[Phone] FROM[Order_Details] INNER JOIN[Order] ON[Order_Details].[OrderId] =[Order].[Order_Id] INNER JOIN[Customer] ON[Customer].[Customer_Id] =[Order].[Customer_Id] INNER JOIN[Products] ON[Products].[ProductId] =[Order_Details].[ProductId]";

            var report = _context.OrderCustomerDetailsClasses.FromSqlRaw(sql).ToList();
            return View(report);
        }
        [Route("Report/CustomerIndex")]
        public IActionResult CustomerIndex()
        {
            return View(new List<WebApplication8.ViewModels.OrderCustomerDetailsClass>());
        }
        [HttpGet]
        [Route("Report/CustomerIndex/{param}")]
        public IActionResult CustomerIndex(string searchString)
        {
            string sql1 = "SELECT [Customer].[First_Name],[Customer].[Last_Name],[Customer].[Customer_Email],[Customer].[Phone],[Order_Details].[OrderId],[Order].[Order_No],[Order].[Order_Date],[Order_Details].[ProductId],[Products].[Name],[Products].[Image],[Order_Details].[Quantity] ,[Order_Details].[Price],[Order_Details].[Subtotal] FROM[Order_Details] INNER JOIN[Order] ON[Order_Details].[OrderId] =[Order].[Order_Id] INNER JOIN[Customer] ON[Customer].[Customer_Id] =[Order].[Customer_Id] INNER JOIN[Products] ON[Products].[ProductId] =[Order_Details].[ProductId] WHERE [Customer].[Phone] LIKE @p0";
            string wrapString = "%" + searchString + "%";

            var report = _context.OrderCustomerDetailsClasses.FromSqlRaw(sql1, wrapString).ToList();
            return View(report);
        }
    }
}