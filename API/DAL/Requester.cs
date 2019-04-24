using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySqlLib;

namespace API.DAL
{
    internal class Requester : ApiGeneral
    {
        public Requester(MySqlData mySql) : base(mySql)
        {
        }

        internal DataTable SelectById<T>(int id)
        {
            string requestPart = ClassToDBName<T>();

            string request = $"SELECT * FROM {requestPart} WHERE id = '{id}'";
            var res = MySql.ExecuteReader(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                return res.Result;
            }
        }

        internal DataTable SelectByLogin<T>(string login)
        {
            string requestPart = ClassToDBName<T>();

            string request = $"SELECT * FROM {requestPart} WHERE login = '{login}'";
            var res = MySql.ExecuteReader(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                return res.Result;
            }
        }

        private string ClassToDBName<T>()
        {
            string nameOfClass = typeof(T).Name.ToString();
            string requestPart = "";
            switch (nameOfClass)
            {
                case "ApiCheck":
                    requestPart = "hotel.check";
                    break;
                case "ApiHotel":
                    requestPart = "hotel.hotel";
                    break;
                case "ApiRequest":
                    requestPart = "hotel.request";
                    break;
                case "ApiRoom":
                    requestPart = "hotel.room";
                    break;
                case "ApiUser":
                    requestPart = "hotel.user";
                    break;
                default:
                    break;
            }
            return requestPart;
        }

        internal DataTable SelectAll<T>()
        {
            string requestPart = ClassToDBName<T>();

            string request = $"SELECT * FROM {requestPart}";
            var res = MySql.ExecuteReader(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                return res.Result;
            }
        }

        internal bool Remove<T>(int id)
        {
            string requestPart = ClassToDBName<T>();

            string request = $"DELETE FROM {requestPart} WHERE id = '{id}'";
            var res = MySql.ExecuteNonQuery(request);
            if (res.HasError)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
