using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Scraper
{
    public class Utilities
    {
        //Converts a month string to a corresponding int value
        public static int GetMonthINT(string monthString)
        {
            int month = -1;

            if (monthString.Equals("January")) month = 1;
            if (monthString.Equals("February")) month = 2;
            if (monthString.Equals("March")) month = 3;
            if (monthString.Equals("April")) month = 4;
            if (monthString.Equals("May")) month = 5;
            if (monthString.Equals("June")) month = 6;
            if (monthString.Equals("July")) month = 7;
            if (monthString.Equals("August")) month = 8;
            if (monthString.Equals("September")) month = 9;
            if (monthString.Equals("October")) month = 10;
            if (monthString.Equals("November")) month = 11;
            if (monthString.Equals("December")) month = 12;

            return month;
        }

        //Converts a month int value to its corresponding month name
        public static string GetMonthString(int month)
        {
            string monthString = "null";

            if (month == 1) monthString = "January";
            if (month == 2) monthString = "February";
            if (month == 3) monthString = "March";
            if (month == 4) monthString = "April";
            if (month == 5) monthString = "May";
            if (month == 6) monthString = "June";
            if (month == 7) monthString = "July";
            if (month == 8) monthString = "August";
            if (month == 9) monthString = "September";
            if (month == 10) monthString = "October";
            if (month == 11) monthString = "November";
            if (month == 12) monthString = "December";

            return monthString;
        }

    }
}
