using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Green.Models
{
    public class Idnumber
    {
        public int GetYear(string idnumber)
        {            
            int year=0;
            year = Convert.ToInt32(idnumber.Substring(0,2));

            if (year > 30) year = Convert.ToInt32("19"+year);

            else year = Convert.ToInt32("20" + year);

            return year;
        }
    }
}
