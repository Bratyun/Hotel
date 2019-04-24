using MySqlLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.DAL
{
    public class ApiGeneral
    {
        protected MySqlData MySql;

        public ApiGeneral(MySqlData mySql)
        {
            MySql = mySql;
        }

        public bool IsValidPhone(string phone)
        {
            Regex regex = new Regex(@"^\s*\+?\s*([0-9][\s-]*){9,}$");
            MatchCollection matches = regex.Matches(phone);
            if (matches.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
            MatchCollection matches = regex.Matches(email);
            if (matches.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsValidUrl(string url)
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(url);
            if (matches.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsInvalidDate(DateTime start, DateTime end)
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

        public static bool IsDefaultDate(DateTime start, DateTime end)
        {
            if (start == default(DateTime) && end == default(DateTime))
            {
                return true;
            }
            return false;
        }
    }
}
