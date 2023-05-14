using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ABCD_Client.Models;
using PagedList;
using PagedList.Mvc;

namespace ABCD_Client.Controllers
{
    public class ProductsController : Controller
    {
        private Entities1 db = new Entities1();

        // GET: Products
        public ActionResult Index(int? i)
        {
            var products = db.Products.Include(p => p.Shop);
            var imagePathsList = new List<string>();
            foreach (var product in products)
            {
                imagePathsList.Add(product.ProductImages.First().imagePath);
            }
            ViewBag.ImagePaths = imagePathsList;
            ViewBag.position = "Products";
            return View(products.ToList().ToPagedList(i ?? 1,8));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var Images = product.ProductImages.ToList();
            ViewBag.Images = Images;
            ViewBag.position = "Products";
            return View(product);
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
