using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Green.Models;
using Green.ViewModels;

namespace Green.Controllers
{
    public class LabsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Labs
        public ActionResult Index()
        {
            return View(db.Labs.ToList());
        }

        // GET: Labs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lab lab = db.Labs.Find(id);
            if (lab == null)
            {
                return HttpNotFound();
            }
            return View(lab);
        }

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult ViewIndex()
        //{
        //    ApplicationDbContext dd = new ApplicationDbContext(); //dbcontect class


        //    List<LabViewModel> CustomerVMlist = new List<LabViewModel>(); // to hold list of Customer and order details
        //    var customerlist = (from Cust in dd.Appointments
        //                        join Ord in dd.Procedures on Cust.ProcedureName equals Ord.ProcedureName
        //                        join lbs in dd.Labs on Cust.ProcedureName equals lbs.ProcedureName
        //                        //join ff in dd.LabPractitioners on lbs.LabID equals ff.LabID
                              

                                
                                
                                
                               

        //                        select new { lbs.LabID, lbs.LabName, Cust.Doctor, Cust.UserID, Cust.Date, Cust.AppointmentID }).ToList();
        //    //query getting data from database from joining two tables and storing data in customerlist
        //    foreach (var item in customerlist)
        //    {
        //        LabViewModel objcvm = new LabViewModel(); // ViewModel
        //        objcvm.DoctorName = item.Doctor.ToString();
        //        objcvm.LabID = item.LabID;
        //        objcvm.LabName = item.LabName;
            
        //        objcvm.UserID = item.UserID;
        //        objcvm.AppointmentID = item.AppointmentID;
           
        //        objcvm.Date = item.Date;

        //        //objcvm.Address = item.Address;
        //        //objcvm.OrderDate = item.OrderDate;
        //        //objcvm.OrderPrice = item.OrderPrice;
        //        CustomerVMlist.Add(objcvm);
        //    }
        //    //Using foreach loop fill data from custmerlist to List<CustomerVM>.
        //    return View(CustomerVMlist); //List of CustomerVM (ViewModel)
        //}

        // GET: Labs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Labs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LabID,LabName")] Lab lab)
        {
            if (ModelState.IsValid)
            {
                db.Labs.Add(lab);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lab);
        }

        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult ViewIndex()
        //{
        //    ApplicationDbContext dd = new ApplicationDbContext(); //dbcontect class
        //    List<LabViewModel> CustomerVMlist = new List<LabViewModel>(); // to hold list of Customer and order details
        //    var customerlist = (from Cust in dd.Appointments 
        //                        join Ord in dd.Procedures on Cust.ProcedureName equals Ord.ProcedureName
        //                        join lbs in dd.Labs on Cust.ProcedureName equals lbs.ProcedureName
                               
        //                        select new { lbs.LabName, Cust.Doctor}).ToList();
        //    //query getting data from database from joining two tables and storing data in customerlist
        //    foreach (var item in customerlist)
        //    {
        //        LabViewModel objcvm = new LabViewModel(); // ViewModel
        //        objcvm.DoctorName = item.Doctor.ToString();
        //        objcvm.LabName = item.LabName;
        //        //objcvm.Address = item.Address;
        //        //objcvm.OrderDate = item.OrderDate;
        //        //objcvm.OrderPrice = item.OrderPrice;
        //        CustomerVMlist.Add(objcvm);
        //    }
        //    //Using foreach loop fill data from custmerlist to List<CustomerVM>.
        //    return View(CustomerVMlist); //List of CustomerVM (ViewModel)
        //}



        // GET: Labs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lab lab = db.Labs.Find(id);
            if (lab == null)
            {
                return HttpNotFound();
            }
            return View(lab);
        }

        // POST: Labs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabID,LabName")] Lab lab)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lab).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lab);
        }

        // GET: Labs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lab lab = db.Labs.Find(id);
            if (lab == null)
            {
                return HttpNotFound();
            }
            return View(lab);
        }

        // POST: Labs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lab lab = db.Labs.Find(id);
            db.Labs.Remove(lab);
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
