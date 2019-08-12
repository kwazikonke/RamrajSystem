using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Green.Models
{
    public class AppointmentModel : IComparable<AppointmentModel>
    {
        [Key]
        public int AppointmentID { get; set; }


        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }//
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required] 
        public int DoctorID { get; set; }//
        public virtual DoctorModel Doctor { get; set; }

        public int ProcedurID { get; set; } // 
        public virtual Procedure Procedure { get; set; }

        [Required]
        [Display(Name = "Date for Appointment")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [MyAppointmentDateValidation(ErrorMessage = "Are you creating an appointment for the past?")]
        public DateTime Date { get; set; }
        
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }        
        public string ProcedureName { get; set; }
        public string TimeBlockHelper { get; set; }        

        public double BookingPrice { get; set; }
        
        public int CompareTo(AppointmentModel other)
        {
            return this.Date.Date.Add(this.Time.TimeOfDay).CompareTo(other.Date.Date.Add(other.Time.TimeOfDay));
        }

       
        //public double GetPrice()
        //{
        //    Procedure ff = new Procedure();
        //    var linP = ff.Price;
        //    return linP;
        //}
        //public double CalcBookingPrice()
        //{
        //    return consultationPrice + GetPrice();
        //}
    }
}