using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ABCD_Client.Models;

namespace ABCD_Client.Controllers
{
    public class CustomersController : Controller
    {
        private Entities1 db = new Entities1();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            Customer customer = db.Customers.FirstOrDefault(c => c.userName == username && c.password == password);

            if (customer != null)
            {
                Session["customerId"] = customer.customerId;
                Session["customerName"] = customer.fullName;

                return RedirectToAction("Index", "Cinema");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Cinema");
        }




        public ActionResult Orders()
        {
            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }
            int customerId = (int)Session["customerId"];
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.PaymentMethod).Where(o => o.customerId == customerId);
            return View(orders.ToList());
        }

        public ActionResult OrderDetails(int orderId)
        {
            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            int customerId = (int)Session["customerId"];

            var order = db.Orders.FirstOrDefault(o => o.orderId == orderId && o.customerId == customerId);

            if (order == null)
            {
                // If the order does not exist or does not belong to this customer, redirect to a error page
                return RedirectToAction("Error", "Home");
            }

            var orderDetails = db.OrderDetails.Where(od => od.orderId == orderId).ToList();

            List<Ticket> tickets = new List<Ticket>();
            foreach (var orderDetail in orderDetails)
            {
                var ticket = db.Tickets.FirstOrDefault(t => t.ticketId == orderDetail.ticketId);
                if (ticket != null)
                {
                    tickets.Add(ticket);
                }
            }

            //ViewBag.Tickets = tickets;

            return View(tickets);
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}