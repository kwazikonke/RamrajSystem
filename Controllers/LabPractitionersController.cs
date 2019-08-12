using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Green.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Green.Controllers
{
    public class LabPractitionersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public LabPractitionersController()
           : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public LabPractitionersController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }





        // GET: LabPractitioners
        public ActionResult Index()
        {
            return View(db.LabPractitioners.ToList());
        }

        // GET: LabPractitioners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabPractitioner labPractitioner = db.LabPractitioners.Find(id);
            if (labPractitioner == null)
            {
                return HttpNotFound();
            }
            return View(labPractitioner);
        }

        // GET: LabPractitioners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LabPractitioners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create([Bind(Include = "LabPractitionerID, LabID,LabPractitionerName,Email,BirthDate,Sex")] LabPractitioner labPractitioner)
        {
            if (ModelState.IsValid)
            {
                var user = labPractitioner.GetUser();
                LabPractitioner dm = new LabPractitioner();
                string password = dm.GeneratePassword();

                var result = await UserManager.CreateAsync(user, password);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                if (result.Succeeded)
                {
                    if (!roleManager.RoleExists("Lab Practitioner"))
                    {
                        roleManager.Create(new IdentityRole("Lab Practitioner"));

                    }
                    await UserManager.AddToRoleAsync(user.Id, "Lab Practitioner");

                    var myMessage = new SendGridMessage
                    {
                        From = new EmailAddress("no-reply@Code-Hub.com", "No-Reply")
                    };
                    myMessage.AddTo(user.Email);
                    string subject = "Registration Received";
                    string body = ("Hi " + user.Name + " " + "\n" + "Your password is" + password + "." +
                    "\n" + " Ensure not to share your password with anyone...  Have a great day." + "\n");
                    myMessage.Subject = subject;
                    myMessage.HtmlContent = body;
                    var transportWeb = new SendGrid.SendGridClient("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");
                    await transportWeb.SendEmailAsync(myMessage);
                    db.LabPractitioners.Add(labPractitioner);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }

            return View(labPractitioner);
        }
        // GET: LabPractitioners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabPractitioner labPractitioner = db.LabPractitioners.Find(id);
            if (labPractitioner == null)
            {
                return HttpNotFound();
            }
            return View(labPractitioner);
        }

        // POST: LabPractitioners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabPractitionerID,LabPractitionerName,LabID,Email,BirthDate,Sex")] LabPractitioner labPractitioner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(labPractitioner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(labPractitioner);
        }

        // GET: LabPractitioners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LabPractitioner labPractitioner = db.LabPractitioners.Find(id);
            if (labPractitioner == null)
            {
                return HttpNotFound();
            }
            return View(labPractitioner);
        }

        // POST: LabPractitioners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LabPractitioner labPractitioner = db.LabPractitioners.Find(id);
            db.LabPractitioners.Remove(labPractitioner);
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
