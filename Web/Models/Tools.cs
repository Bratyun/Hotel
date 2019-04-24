using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Hotel.Models
{
    public static class Tools
    {
        public static bool IsCorrectPhoneNumber(string phoneNumber)
        {
            Regex regex = new Regex(@"\d{3}-\d{3}-\d{4}");
            MatchCollection collection = regex.Matches(phoneNumber);
            return collection.Count == 1;
        }

        public static bool IsDefaultDate(DateTime start, DateTime end)
        {
            if (start == default(DateTime) && end == default(DateTime))
            {
                return true;
            }
            return false;
        }

        public static bool IsInvalidDate(DateTime start, DateTime end)
        {
            if (IsDefaultDate(start, end))
            {
                return true;
            }

            if (((end.Year * 365 + end.Month * 31 + end.Day) - (start.Year * 365 + start.Month * 31 + start.Day)) > 365)
            {
                return true;
            }

            return (start.Date >= end.Date || end.Date < DateTime.Now || start.Date < DateTime.Now.Date) ? true : false;
        }
    }
}