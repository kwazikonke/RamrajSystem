using System;
using System.ComponentModel.DataAnnotations;

namespace Green.Models
{
    
    public class DateRangeAttribute : RangeAttribute
    {
        public DateRangeAttribute()
            : base(typeof(DateTime), DateTime.Now.ToShortDateString(), "1/04/2019")
        {

        }
    }
}
