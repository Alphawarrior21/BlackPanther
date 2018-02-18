using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraderPerformanceComparer.Assets
{
    public static class DateTimeHelper
    {
        public static string getDateTimeStrFromNSEEpochSeconds(int seconds, string dateStrFormat)
        {
            // yyMMM / ddMMMYYYY
            DateTime dt = new DateTime(1980, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
            return dt.ToString(dateStrFormat).ToUpper();
        }

        public static string getDateTimeStrFromUnixEpochSeconds(int seconds, string dateStrFormat)
        {
            // yyMMM / ddMMMYYYY
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
            return dt.ToString(dateStrFormat).ToUpper();
        }

        public static string Day_Month_CustomDate(string date)
        {

            return date.Substring(0, 6);
        }

        public static int ReturnTimeInInteger(string time)
        {
            return Convert.ToInt32(TimeSpan.Parse(time).TotalSeconds);
        }


        public static string ReturnDate_from_DDD_MMM_YYYY(string datetime)
        {
           
            return datetime.Substring(0, 3) + "," + datetime.Substring(8, 2) + " " + datetime.Substring(4, 3) + " " + datetime.Substring(20, 4) + " " + datetime.Substring(11, 8);

        }

        

        public static int calculateSeconds(DateTime dateTimeValue)
        {
            return Convert.ToInt32(dateTimeValue.Subtract(new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Local)).TotalSeconds);
        }


    }
}
