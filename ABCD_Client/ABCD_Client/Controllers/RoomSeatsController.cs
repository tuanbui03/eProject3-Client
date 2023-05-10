using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ABCD_Client.Models;

namespace ABCD_Client.Controllers
{
    public class RoomSeatsController : Controller
    {
        private Entities db = new Entities();

        // GET: RoomSeats
        public ActionResult Index()
        {
            var roomSeats = db.RoomSeats.Include(r => r.Rooms).Include(r => r.Seats);
            return View(roomSeats.ToList());
        }

        // GET: RoomSeats/Details/5
        public ActionResult Details(int? roomId, int? screeningId)
        {
            if (roomId == null || screeningId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve all RoomSeat objects associated with the Room
            List<RoomSeats> roomSeats = db.RoomSeats.Include(s => s.Seats).Where(rs => rs.roomId == roomId).ToList();

            // Retrieve the Screening object with the specified ID
            Screening screening = db.Screening.Include(m => m.Movies).FirstOrDefault(s => s.screeningId == screeningId);

            if (screening == null)
            {
                return HttpNotFound();
            }

            // Pass the RoomSeats and Screening objects to the view using ViewBag
            ViewBag.RoomSeats = roomSeats;
            ViewBag.Screening = screening;

            return View();
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
