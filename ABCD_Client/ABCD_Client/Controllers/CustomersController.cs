using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml.Linq;
using ABCD_Client.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userName,password,email,fullName,birthDate,cardNumber")] Customer customers)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingCustomer = db.Customers.FirstOrDefault(c => c.userName == customers.userName);
                if (existingCustomer != null)
                {
                    ModelState.AddModelError("userName", "Username already exists.");
                    return View(customers);
                }

                db.Customers.Add(customers);
                db.SaveChanges();

                Session["customerId"] = customers.customerId;
                Session["customerName"] = customers.fullName;

                return RedirectToAction("Index", "Cinema");
            }

            return View(customers);
        }


        public ActionResult CustomerProfile()
        {
            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }
            int customerId = (int)Session["customerId"];
            Customer customer = db.Customers.Find(customerId);
            return View(customer);
        }


        [HttpPost]
        public ActionResult CustomerProfile(FormCollection form)
        {
            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            int customerId = (int)Session["customerId"];
            Customer customer = db.Customers.Find(customerId);
            if (customer == null)
            {
                return HttpNotFound();
            }

            string oldPassword = form["OldPassword"];
            string newPassword = form["NewPassword"];

            if (customer.password != oldPassword)
            {
                ModelState.AddModelError(nameof(oldPassword), "Incorrect old password.");
                return View(customer);
            }

            customer.password = newPassword;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Your password has been successfully changed.";

            return RedirectToAction("CustomerProfile", "Customers");
        }

        [HttpGet]
        public JsonResult CheckUserName(string userName)
        {
            bool exists = db.Customers.Any(c => c.userName == userName);
            return Json(new { exists = exists }, JsonRequestBehavior.AllowGet);
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

        public FileResult ExportPdf(int ticketId)
        {
            var ticket = db.Tickets.FirstOrDefault(t => t.ticketId == ticketId);

            // Create a new PDF document with a custom page size
            var document = new Document(new Rectangle(250f, 150f));
            var ms = new MemoryStream();
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Add some content to the document
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var fieldFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            var padding = new Paragraph(" ");

            document.Add(padding);
            document.Add(new Paragraph($"Movie Title: {ticket.Movy.movieTitle}", titleFont));
            document.Add(padding);
            document.Add(new Paragraph($"Room ID: {ticket.RoomSeat.roomId}   Seat: {ticket.seatName}", fieldFont));
            document.Add(new Paragraph($"Reserved Time: {ticket.Screening.reservedTime}", fieldFont));
            document.Add(padding);
            document.Add(new Paragraph($"Ticket Code: {ticket.TicketCode}", titleFont));

            // Close the document
            document.Close();

            // Return the PDF data as a FileResult
            return File(ms.ToArray(), "application/pdf", $"{ticket.TicketCode}.pdf");
        }

        public ActionResult GetCartCount(int customerId)
        {
            int count = db.Carts.Where(c => c.customerId == customerId).Count();
            return Content(count.ToString());
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