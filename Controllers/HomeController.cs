using Green.Models;
using System.Linq;
using System.Web.Mvc;
using Green.ViewModels;
using System.Collections.Generic;

namespace Green.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            IQueryable<AppointmentDateGroupViewModel> data = from appointment in db.Appointments
                                                             group appointment by appointment.Date into dateGroup
                                                             select new AppointmentDateGroupViewModel()
                                                             {
                                                                 AppointmentDate = dateGroup.Key,
                                                                 PatientCount = dateGroup.Count()
                                                             };
            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult HomePage()
        {

            return View();
        }


        //[AcceptVerbs(HttpVerbs.Get)]
        //public ActionResult ViewIndex()
        //{
        //    ApplicationDbContext dd = new ApplicationDbContext(); //dbcontect class


        //    List<PatientRecordViewModel> CustomerVMlist = new List<PatientRecordViewModel>(); // to hold list of Customer and order details
        //    var customerlist = (from Cust in dd.Appointments
        //                        join Ord in dd.Users on Cust.UserID equals Ord.Id


        //                        select new { Cust.UserID, Ord.Name, Cust.Doctor, Cust.ProcedureName, Cust.Date, Cust.Time, Cust.BookingPrice }).ToList();
        //    //query getting data from database from joining two tables and storing data in customerlist
        //    foreach (var item in customerlist)
        //    {
        //        PatientRecordViewModel objcvm = new PatientRecordViewModel(); // ViewModel
        //        objcvm.DoctorName = item.Doctor.ToString();
        //        objcvm.UserID = item.UserID;
        //        objcvm.UserName = item.Name;
        //        objcvm.procedureType = item.ProcedureName;
        //        objcvm.Date = item.Date.ToString();
        //        objcvm.Time = item.Time.ToString();
        //        objcvm.TotalPrice = item.BookingPrice;


        //        //objcvm.Address = item.Address;
        //        //objcvm.OrderDate = item.OrderDate;
        //        //objcvm.OrderPrice = item.OrderPrice;
        //        CustomerVMlist.Add(objcvm);
        //    }
        //    //Using foreach loop fill data from custmerlist to List<CustomerVM>.
        //    return View(CustomerVMlist); //List of CustomerVM (ViewModel)
        //}



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}