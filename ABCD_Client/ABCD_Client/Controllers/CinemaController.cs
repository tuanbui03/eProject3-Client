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







        // GET: Cinema/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Cinema/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cinema/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cinema/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Cinema/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Cinema/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Cinema/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
