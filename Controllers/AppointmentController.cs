using Green.Models;
using Green.ViewModels;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Green.Controllers
{
    public class AppointmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

       
        // GET: /Appointments/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentModel appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return View("Error");
            }
            return View(appointment);
        }

        // GET: /Appointments/Create
        //[Authorize(Roles = "Patient")]
        public ActionResult Create(int? id)
        {
            Session["docId"] = id;
            ViewBag.Prod = new SelectList(db.Procedures.Where(x => x.ProcedureName == "Consultation"), "ProcedurID", "ProcedureName");
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name");
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            DoctorModel doctor = db.Doctors.Find(id);
            var model = new AppointmentModel
            {
                DoctorID = Convert.ToInt32(id),
                UserID = User.Identity.GetUserId()
            };
            ViewBag.DoctorName = doctor.Name;
            ViewBag.Picture = doctor.Picture;
            return View(model);
        }

        public ActionResult DentistCreate(string id)
        {
            //int docId;
            //Session["docId"] = docId ;
            ViewBag.Prod = new SelectList(db.Procedures, "ProcedurID", "ProcedureName");
            ViewBag.PatientID = new SelectList(db.Patients, "ID", "Name");
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            var pat = db.Users.Find(id);

            //var Vloan = (from ltable in db.Users
            //             join ctable in db.Doctors on ltable.Id equals ctable.UserID
            //            // where ctable.
            //             select ctable).FirstOrDefault();

            var current = db.Users.Find(User.Identity.GetUserId());
            var docid = db.Doctors.ToList().Find(x => x.Email == current.Email);

            Session["docId"] = docid.DoctorID;
            Session["UserId"] = id;
            var model = new AppointmentModel
            {
                UserID = id,
                DoctorID = docid.DoctorID,

            };
            ViewBag.DoctorName = db.Users.Find(User.Identity.GetUserId()).Name;
            ViewBag.Picture = db.Users.Find(id).Name;
            return View("DentistCreate", model);
        }



        // POST: /Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AppointmentID,DoctorID,Date,TimeBlockHelper,Time,ProcedurID,ProcedureName, BookingPrice")] AppointmentModel appointment)
        {
            appointment.ProcedureName = db.Procedures.Find(appointment.ProcedurID).ProcedureName;
            appointment.BookingPrice = db.Procedures.Find(appointment.ProcedurID).Price;


            //Add userID
            appointment.UserID = User.Identity.GetUserId();
            var doctor = Session["docId"].ToString();
            appointment.DoctorID = int.Parse(doctor);
            //Verify Time
            if (appointment.TimeBlockHelper == "DONT")
                ModelState.AddModelError("", "No Appointments Available for " + appointment.Date.ToShortDateString());
            else
            {
                //Set Time
                appointment.Time = DateTime.Parse(appointment.TimeBlockHelper);

                //CheckWorkingHours
                DateTime start = appointment.Date.Add(appointment.Time.TimeOfDay);
                DateTime end = (appointment.Date.Add(appointment.Time.TimeOfDay)).AddMinutes(double.Parse(db.Administrations.Find(1).Value));
                if (!(BusinessLogic.IsInWorkingHours(start, end)))
                    ModelState.AddModelError("", "Doctor Working Hours are from " + int.Parse(db.Administrations.Find(2).Value) + " to " + int.Parse(db.Administrations.Find(3).Value));

                //Check Appointment Clash
                string check = BusinessLogic.ValidateNoAppoinmentClash(appointment);
                if (check != "")
                    ModelState.AddModelError("", check);
            }

            //Continue Normally
            if (ModelState.IsValid)
            {
                var myMessage = new SendGridMessage
                {
                    From = new EmailAddress("no-reply@Code-Hub.com", "No-Reply")
                };
                var u = db.Users.Find(User.Identity.GetUserId());
                myMessage.AddTo(u.Email);
                string subject = "Appointement Booking Received";
                string body = ("Hi " + u.Name + " " + "\n" + "Your Appointment At " + "<b>" + appointment.Time + "</b>" + " ." +
                "\n" + "On" + appointment.Date + "\n");
                myMessage.Subject = subject;
                myMessage.HtmlContent = body;
                var transportWeb = new SendGrid.SendGridClient("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");
                await transportWeb.SendEmailAsync(myMessage);


                //  appointment.BookingPrice = appointment.CalcBookingPrice();
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("Details", new { Controller = "RegisteredUsers", Action = "Details" });
            }

            //Fill Neccessary ViewBags
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name", appointment.DoctorID);
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            return View(appointment);
        }


        [HttpPost]
        //[Authorize(Roles = "Patient")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DentistCreate([Bind(Include = "AppointmentID,DoctorID,Date,TimeBlockHelper,Time,ProcedurID,ProcedureName, BookingPrice")] AppointmentModel appointment)
        {
            appointment.ProcedureName = db.Procedures.Find(appointment.ProcedurID).ProcedureName;
            appointment.BookingPrice = db.Procedures.Find(appointment.ProcedurID).Price;
            //Add userID
            // var current = db.Users.Find(User.Identity.GetUserId());
            //var docid = db.Doctors.ToList().Find(x => x.Email == current.Email).DoctorID;
            //  appointment.DoctorID = Convert.ToInt32(User.Identity.GetUserId());
            //   var doctor = Session["docId"].ToString();
            // appointment.DoctorID = Convert.ToInt32();
            appointment.DoctorID = (int)Session["docId"];
            appointment.UserID = (string)Session["UserId"];
            //Verify Time
            if (appointment.TimeBlockHelper == "DONT")
                ModelState.AddModelError("", "No Appointments Available for " + appointment.Date.ToShortDateString());
            else
            {
                //Set Time
                appointment.Time = DateTime.Parse(appointment.TimeBlockHelper);

                //CheckWorkingHours
                DateTime start = appointment.Date.Add(appointment.Time.TimeOfDay);
                DateTime end = (appointment.Date.Add(appointment.Time.TimeOfDay)).AddMinutes(double.Parse(db.Administrations.Find(1).Value));
                if (!(BusinessLogic.IsInWorkingHours(start, end)))
                    ModelState.AddModelError("", "Doctor Working Hours are from " + int.Parse(db.Administrations.Find(2).Value) + " to " + int.Parse(db.Administrations.Find(3).Value));

                //Check Appointment Clash
                string check = BusinessLogic.ValidateNoAppoinmentClash(appointment);
                if (check != "")
                    ModelState.AddModelError("", check);
            }

            //Continue Normally
            if (ModelState.IsValid)
            {

                var myMessage = new SendGridMessage
                {
                    From = new EmailAddress("no-reply@Code-Hub.com", "No-Reply")
                };
              var u =  db.Users.Find((string)Session["UserId"]);
                myMessage.AddTo(u.Email);
                string subject = "Appointement Booking Received";
                string body = ("Hi " + u.Name + " " + "\n" + "Your Appointment At " + "<b>" + appointment.Time + "</b>" + " ." +
                "\n" + "On" + appointment.Date + "\n");
                myMessage.Subject = subject;
                myMessage.HtmlContent = body;
                var transportWeb = new SendGrid.SendGridClient("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");
                await transportWeb.SendEmailAsync(myMessage);


                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("UpcomingAppointments", "Doctor");
            }
            //Fill Neccessary ViewBags
            ViewBag.PatientID = new SelectList(db.Patients, "ID", "Name", appointment.UserID);
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            return View(appointment);
        }




        // GET: /Appointments/Edit/5
        //[Authorize(Roles = "Admin, Patient")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentModel appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return View("Error");
            }
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            ViewBag.Date = appointment.Date.Date;
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name", appointment.DoctorID);
            return View(appointment);
        }

        // POST: /Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "AppointmentID,DoctorID,Date,TimeBlockHelper,Available")] AppointmentModel appointment)
        {
            //Verify Time
            if (appointment.TimeBlockHelper == "DONT")
                ModelState.AddModelError("", "No Appointments Available for " + appointment.Date.ToShortDateString());
            else
            {
                //Set Time
                appointment.Time = DateTime.Parse(appointment.TimeBlockHelper);
                //Check WorkingHours
                DateTime start = new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, appointment.Time.Hour, appointment.Time.Minute, appointment.Time.Second);
                DateTime end = new DateTime(appointment.Date.Year, appointment.Date.Month, appointment.Date.Day, appointment.Time.Hour, appointment.Time.Minute, appointment.Time.Second).AddMinutes(double.Parse(db.Administrations.Find(1).Value));
                if (!BusinessLogic.IsInWorkingHours(start, end))
                    ModelState.AddModelError("", "Doctor Working Hours are from " + int.Parse(db.Administrations.Find(2).Value) + " to " + int.Parse(db.Administrations.Find(3).Value));
            }

            //Continue
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.Entry(appointment).Property(u => u.UserID).IsModified = false;
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Details", new { Controller = "RegisteredUsers", Action = "Details" });

            }
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            ViewBag.DoctorID = new SelectList(db.Doctors.Where(x => x.DisableNewAppointments == false), "ID", "Name", appointment.DoctorID);
            return View(appointment);
        }

        // GET: /Appointments/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentModel appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return View("Error");
            }
            return View(appointment);
        }


        public ActionResult PatientIndex(string searchstring)
        {
            var pat = from d in db.Patients
                      select d;
            if (!string.IsNullOrEmpty(searchstring))
            {
                pat = pat.Where(s => s.Name.Contains(searchstring));
            }
            return View(pat);
        }



        // POST: /Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            AppointmentModel appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index");
            else if (User.IsInRole("Doctor"))
                return RedirectToAction("Details", new { Controller = "Doctor", Action = "UpcomingAppointments", id = User.Identity.Name });
            else if (User.IsInRole("Patient"))
                return RedirectToAction("Details", new { Controller = "RegisteredUsers", Action = "Details" });
            else
                return RedirectToAction("Index", new { Controller = "Home", Action = "Index" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Here or in model?
        [HttpPost]
        public JsonResult GetAvailableAppointments(int docID, DateTime date)
        {
            List<SelectListItem> resultlist = BusinessLogic.AvailableAppointments(docID, date);
            return Json(resultlist);
        }
    }
}
