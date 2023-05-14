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

namespace ABCD_Client.Controllers
{
    public class ShopsController : Controller
    {
        private Entities1 db = new Entities1();

        // GET: Shops
        public ActionResult Index(int? i)
        {
            ViewBag.position = "Shops";
            return View(db.Shops.ToList().ToPagedList(i ?? 1, 8));
        }

        // GET: Shops/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shops = db.Shops.Find(id);
            if (shops == null)
            {
                return HttpNotFound();
            }
            var feedBacks = db.Feedbacks.Where(f => f.shopId == id).Include(c => c.Customer).ToList();
            ViewBag.Feedbacks = feedBacks;
            ViewBag.position = "Shops";
            return View(shops);
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
