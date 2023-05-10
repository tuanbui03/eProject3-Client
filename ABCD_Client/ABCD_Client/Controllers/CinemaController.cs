﻿using System;
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
        private Entities db = new Entities();
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

            var screenings = db.Screening
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
            return View();
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
