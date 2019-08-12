using Green.Models;
using Green.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Green.Controllers
{
    // GET: Admin
  //  [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ActionResult dashboard()
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
    }
}