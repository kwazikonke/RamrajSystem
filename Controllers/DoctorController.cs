
using Green.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Green.Controllers
{
    public class DoctorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public DoctorController()
           : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public DoctorController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        // GET: /Doctors/
        public ActionResult Index(string docDept, string searchstring)
        {
            var deptList = new List<Enums.DepartmentDentistry>();
            deptList = Enum.GetValues(typeof(Enums.DepartmentDentistry))
                                    .Cast<Enums.DepartmentDentistry>()
                                    .ToList();
            ViewBag.docDept = new SelectList(deptList);

            var doctors = from d in db.Doctors
                          select d;

            if (!string.IsNullOrEmpty(searchstring))
            {
                doctors = doctors.Where(s => s.Name.Contains(searchstring));
            }

            if (!string.IsNullOrEmpty(docDept))
            {
                doctors = doctors.Where(x => x.Department.ToString() == docDept);
            }
            return View(doctors);
        }

        //Doctor Availability
        public ActionResult Availability(int? id)
        {
            DoctorModel doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return View("Error");
            }
            AppointmentModel test = new AppointmentModel
            {
                DoctorID = Convert.ToInt32(id),
            };
            ViewBag.TimeBlockHelper = new SelectList(String.Empty);
            ViewBag.DoctorName = doctor.Name;
            return View(test);
        }

        //GET: /Doctors/UpcomingAppointments/5
        [Authorize(Roles = "Admin, Doctor")]
        public ActionResult UpcomingAppointments(string SearchString)
        {
            int n;
          //  bool isInt = int.TryParse(id, out n);
            //if (!isInt)
            //{
                var user = db.Users.Find(User.Identity.GetUserId());
                var model = new EditUserViewModel(user);
                DoctorModel doctor = db.Doctors.First(u => u.Name == user.Name);
                if (doctor == null)
                {
                    return View("Error");
                }
                if (!String.IsNullOrEmpty(SearchString))
                {
                    doctor.Appointments = doctor.Appointments.Where(s => s.ApplicationUser.Name.ToUpper().Contains(SearchString.ToUpper())).ToList();
                }
                doctor.Appointments.Sort();
                return View(doctor);
           // }
            //else
            //{
            //    if (!User.IsInRole("Admin"))
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    DoctorModel doctor = db.Doctors.Find(n);
            //    if (doctor == null)
            //    {
            //        return View("Error");
            //    }
            //    doctor.Appointments.Sort();
            //    return View(doctor);
            //}
        }

        // GET: /Doctors/History
        [Authorize(Roles = "Admin, Doctor")]
        public ActionResult History(string id)
        {
            int n;
            bool isInt = int.TryParse(id, out n);
            if (!isInt)
            {
                var user = db.Users.First(u => u.Email == id);
                var model = new EditUserViewModel(user);
                DoctorModel doctor = db.Doctors.First(u => u.Name == user.Name);
                if (doctor == null)
                {
                    return View("Error");
                }
                doctor.Appointments.Sort();
                return View(doctor);
            }
            else
            {
                if (!User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DoctorModel doctor = db.Doctors.Find(n);
                if (doctor == null)
                {
                    return View("Error");
                }
                doctor.Appointments.Sort();
                return View(doctor);
            }
        }

        public ActionResult AllDoctors()
        {
            return View(db.Doctors.ToList());
        }

        // GET: /Doctors/Create
        // [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more UpcomingAppointments see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //  [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Email,UserId,Name,BirthDate,Sex,Department,Degree,Picture")] DoctorModel doctor, HttpPostedFileBase img_upload)
        {
            byte[] data = null;
            data = new byte[img_upload.ContentLength];
            img_upload.InputStream.Read(data, 0, img_upload.ContentLength);
            doctor.Picture = data;

            if (ModelState.IsValid)
            {
                var user = doctor.GetUser();
                DoctorModel dm = new DoctorModel();
              
                string password = dm.GeneratePassword();

                var result = await UserManager.CreateAsync(user, password);
               
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
               // dm.UserId = user.Id;
                if (result.Succeeded)
                {

                    if (!roleManager.RoleExists("Doctor"))
                    {
                        roleManager.Create(new IdentityRole("Doctor"));
                    }
                    await  UserManager.AddToRoleAsync(user.Id,"Doctor");

                    var myMessage = new SendGridMessage
                    {
                        From = new EmailAddress("no-reply@Code-Hub.com", "No-Reply")
                    };
                   
                    myMessage.AddTo(user.Email);
                    string subject = "Registration Received";
                    string body = ("Hi " + user.Name + " " + "\n" + "Your password is  "  + "<b>" +  password + "</b>" + "  ." +
                    "\n" + " Ensure not to share your password with anyone...  Have a great day." + "\n");
                    myMessage.Subject = subject;
                    myMessage.HtmlContent = body;
                    var transportWeb = new SendGrid.SendGridClient("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");
                    await transportWeb.SendEmailAsync(myMessage);
                   
                    db.Doctors.Add(doctor);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
               
            }

            return View(doctor);
        }



        // GET: /Doctors/Edit/5
        [Authorize(Roles = "Admin, Doctor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return View("Error");
            }
            return View(doctor);
        }

        // POST: /Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more UpcomingAppointments see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Doctor")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email,BirthDate,Sex,Department,Degree,DisableNewAppointments,Picture")] DoctorModel doctor, HttpPostedFileBase img_upload)
        {
            byte[] data = null;
            data = new byte[img_upload.ContentLength];
            img_upload.InputStream.Read(data, 0, img_upload.ContentLength);
            doctor.Picture = data;

            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: /Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorModel doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return View("Error");
            }
            return View(doctor);
        }

        // POST: /Doctors/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorModel doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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
