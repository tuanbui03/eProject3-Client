using ABCD_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ABCD_Client.Controllers
{
    public class HomeController : Controller
    {
        private Entities1 db = new Entities1();

          public ActionResult Login()
        {
            ViewBag.position = "Home";
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

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                ViewBag.position = "Home";
                return View();
            }
        }

        public ActionResult Index()
        {
            var shops = db.Shops.Take(4).ToList();
            var products = db.Products.Take(4).ToList();
            var imagePathsList = new List<string>();
            foreach (var product in products)
            {
                imagePathsList.Add(product.ProductImages.First().imagePath);
            }
            var movies = db.Movies.Take(4).ToList();

            ViewBag.Shops = shops;
            ViewBag.Products = products;
            ViewBag.Movies = movies;
            ViewBag.ImagePaths = imagePathsList;
            ViewBag.position = "Home";
            return View();
        }


        public ActionResult Shop()
        {
            var shops = db.Shops.ToList();
            ViewBag.position = "Home";
            return View(shops);
        }

        public ActionResult About()
        {
            ViewBag.position = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.position = "Contact";
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}