using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ABCD_Client.Models;

namespace ABCD_Client.Controllers
{
    public class CinemaController : Controller
    {
        private Entities1 db = new Entities1();
        // GET: Cinema
        public ActionResult Index()
        {
            var movies = db.Movies.ToList();

            return View(movies);
        }


        public ActionResult BookingTickets(int id)
        {
            var movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            var screenings = db.Screenings
                .Where(s => s.movieId == id && s.reservedTime >= DateTime.Now)
                .OrderBy(s => s.reservedTime)
                .ToList();

            ViewBag.Screenings = screenings;

            return View(movie);
        }


        public ActionResult AddToCart(int? roomId, int? screeningId)
        {
            if (roomId == null || screeningId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            // Retrieve all RoomSeat objects associated with the Room
            List<RoomSeat> roomSeats = db.RoomSeats.Include(s => s.Seat).Where(rs => rs.roomId == roomId).ToList();

            // Retrieve the Screening object with the specified ID
            Screening screening = db.Screenings.Include(m => m.Movy).FirstOrDefault(s => s.screeningId == screeningId);

            if (screening == null)
            {
                return HttpNotFound();
            }

            // Query the Tickets by the given screeningId and get the seatIds that have already been reserved
            var reservedSeats = db.Tickets.Where(t => t.screeningId == screeningId).Select(t => t.seatId).ToList();

            // Disable the seats that have already been reserved
            foreach (var roomSeat in roomSeats)
            {
                if (reservedSeats.Contains(roomSeat.seatId))
                {
                    roomSeat.isAvailable = false;
                }
            }

            // Pass the RoomSeats and Screening objects to the view using ViewBag
            ViewBag.RoomSeats = roomSeats;
            ViewBag.Screening = screening;

            return View();
        }

        [HttpPost]
        public ActionResult CreateMultipleTickets(List<Ticket> tickets, List<Cart> carts)
        {
            if (ModelState.IsValid)
            {
                int customerId = (int)Session["customerId"];
                var i = 0;
                foreach (var ticket in tickets)
                {   
                    var cart = carts[i];
                        db.Tickets.Add(ticket);
                        db.SaveChanges(); // Save changes after adding each Ticket object

                        // Get the generated Id value
                        int generatedId = db.Tickets.Local.Last().ticketId;

                        // Add a corresponding Cart object to the database
                        cart.ticketId = generatedId;

                        db.Carts.Add(cart);
                        db.SaveChanges(); // Save changes after adding the Cart object
                    i++;
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }






        [HttpPost]
        public ActionResult SearchMovies(string query)
        {
            var movies = db.Movies.Where(m => m.movieTitle.Contains(query)).ToList();
            string html = "";

            foreach (var movie in movies)
            {
                html += "<div>";
                html += $"<a href='{Url.Action("BookingTickets", "Cinema", new { id = movie.movieId })}'><img src='{Url.Content("~/img/movie/" + movie.imagePath)}' class='w-100' alt='{movie.movieTitle}'></a>";
                html += $"<h4>{movie.movieTitle}</h4>";
                html += "</div>";
            }

            return Content(html);
        }

        public ActionResult Cart()
        {
            if (Session["customerId"] == null)
            { 
                return RedirectToAction("Login", "Customers");
            }
            int customerId = (int)Session["customerId"];
            var carts = db.Carts.Include(t => t.Ticket)
                                 .Include(m => m.Ticket.Movy)
                                 .Include(m => m.Ticket.Screening)
                                 .Where(c => c.customerId == customerId)
                                 .ToList();
            ViewBag.carts = carts;
            return View();
        }

        public ActionResult RemoveFromCart(int customerId, int ticketId)
        {
            var cartItem = db.Carts.SingleOrDefault(c => c.customerId == customerId && c.ticketId == ticketId);
            if (cartItem != null)
            {
                db.Carts.Remove(cartItem);
                db.SaveChanges();
            }

            var ticketItem = db.Tickets.SingleOrDefault(t => t.ticketId == ticketId);
            if (ticketItem != null)
            {
                db.Tickets.Remove(ticketItem);
                db.SaveChanges();
            }

            return RedirectToAction("Cart");
        }

        public ActionResult PaymentCheckOut()
        {
            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            var payments = db.PaymentMethods.ToList();
            var paymentSelectList = new SelectList(payments, "paymentId", "paymentName");

            ViewBag.PaymentMethods = paymentSelectList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(int paymentId)
        {
            if (Session["customerId"] == null)
            {
                return RedirectToAction("Login", "Customers");
            }

            int customerId = (int)Session["customerId"];

            var cartItems = db.Carts
                            .Where(c => c.customerId == customerId)
                            .Join(db.Tickets,
                                  cart => cart.ticketId,
                                  ticket => ticket.ticketId,
                                  (cart, ticket) => new { TicketId = ticket.ticketId, Price = cart.price })
                            .ToList();
            if (cartItems.Count == 0)
            {
                return RedirectToAction("Cart", "Cinema");
            }
            decimal totalPrice = cartItems.Sum(c => c.Price);

            // Create a new order object and populate its properties
            Order order = new Order();
            order.customerId = customerId;
            order.paymentId = paymentId;
            order.totalPrice = (int)totalPrice;
            order.isConfirm = false;
            order.isPurchased = true;
            order.bookingDate = DateTime.Now;

            // Save the new order to the database and get its ID
            db.Orders.Add(order);
            db.SaveChanges();
            int orderId = order.orderId;

            // Create order details for each ticket in the cart
            foreach (var cartItem in cartItems)
            {
                int ticketId = cartItem.TicketId;
                int ticketPrice = (int)cartItem.Price;
                db.InsertOrderDetail(orderId, ticketId, ticketPrice);
            }


            // Remove all cart items for the customer
            var cartItemsToRemove = db.Carts.Where(c => c.customerId == customerId).ToList();
            foreach (var cartItem in cartItemsToRemove)
            {
                db.Carts.Remove(cartItem);
            }
            db.SaveChanges();

            return RedirectToAction("Index", "Cinema");
        }




    }
}
