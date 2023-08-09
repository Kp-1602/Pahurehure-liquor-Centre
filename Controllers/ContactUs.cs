using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web;
using WebApplication8.Models;
using System.Net;

namespace WebApplication8.Controllers
{
    public class ContactUs : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Contact vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var fromAddress = new MailAddress("ashus7053@gmail.com", "From Name");
                    var toAddress = new MailAddress("ashus7053@gmail.com", "To Name");
                    const string fromPassword = "GulGulmememe878787@@@";
                    string subject = vm.Subject;
                    string body = vm.Message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                        
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);
                    }
                    ModelState.Clear();
                    ViewBag.Message = "Thank you for Contacting us ";
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                }
            }
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
