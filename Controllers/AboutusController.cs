using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication8.Controllers
{
    public class AboutusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
