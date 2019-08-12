using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Green.Models;

namespace Green.Controllers
{
    public class MyRoomsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyRooms
        public ActionResult Index()
        {
            return View(db.MyRooms.ToList());
        }

        // GET: MyRooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyRooms myRooms = db.MyRooms.Find(id);
            if (myRooms == null)
            {
                return HttpNotFound();
            }
            return View(myRooms);
        }

        // GET: MyRooms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MyRooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RoomID,Category")] MyRooms myRooms)
        {
            if (ModelState.IsValid)
            {
                db.MyRooms.Add(myRooms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(myRooms);
        }

        // GET: MyRooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyRooms myRooms = db.MyRooms.Find(id);
            if (myRooms == null)
            {
                return HttpNotFound();
            }
            return View(myRooms);
        }

        // POST: MyRooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RoomID,Category")] MyRooms myRooms)
        {
            if (ModelState.IsValid)
            {
                db.Entry(myRooms).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(myRooms);
        }

        // GET: MyRooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyRooms myRooms = db.MyRooms.Find(id);
            if (myRooms == null)
            {
                return HttpNotFound();
            }
            return View(myRooms);
        }

        // POST: MyRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MyRooms myRooms = db.MyRooms.Find(id);
            db.MyRooms.Remove(myRooms);
            db.SaveChanges();
            return RedirectToAction("Index");
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
