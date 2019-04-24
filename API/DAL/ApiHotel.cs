using API.Interfaces;
using Contract.Models;
using MySqlLib;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace API.DAL
{
    public class ApiHotel : ApiGeneral, IWorkWithDB<Hotel>
    {
        public ApiHotel(MySqlData mySql) : base(mySql)
        {
        }

        public Hotel Create(Hotel obj)
        {
            if (!IsValidPhone(obj.Phone) || !IsValidUrl(obj.Site))
            {
                return null;
            }
            string request = $"INSERT INTO hotel.hotel (address, phone, star, site, image) VALUES (" +
                $"'{obj.Address}', '{obj.Phone}', '{obj.Star}', '{obj.Site}', '{Encoding.UTF8.GetString(obj.Image)}')";
            var res = MySql.ExecuteNonQuery(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                var toReturn = MySql.ExecuteReader($"SELECT * FROM hotel.hotel WHERE address = '{obj.Address}' AND phone = '{obj.Phone}' AND star = '{obj.Star}' " +
                    $"AND site = '{obj.Site}' AND image = '{Encoding.UTF8.GetString(obj.Image)}'");
                if (toReturn.HasError)
                {
                    return null;
                }
                else
                {
                    foreach (DataRow row in toReturn.Result.Rows)
                    {
                        Hotel r = new Hotel
                        {
                            Id = row.Get<int>("id"),
                            Address = row.Get<string>("address"),
                            Phone = row.Get<string>("phone"),
                            Star = row.Get<int>("star"),
                            Site = row.Get<string>("site"),
                            Image = Encoding.UTF8.GetBytes(row.Get<string>("image"))
                        };
                        return r;
                    }
                    return null;
                }
            }
        }

        public bool Delete(int id)
        {
            Requester requester = new Requester(MySql);
            return requester.Remove<ApiHotel>(id);
        }

        public List<Hotel> GetAll()
        {
            return GetAll(HotelSortBy.None, false);
        }

        public List<Hotel> GetAll(HotelSortBy sortBy, bool desc)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectAll<ApiHotel>();
            if (table == null)
            {
                return null;
            }

            List<Hotel> requests = new List<Hotel>();
            foreach (DataRow row in table.Rows)
            {
                Hotel r = new Hotel
                {
                    Id = row.Get<int>("id"),
                    Address = row.Get<string>("address"),
                    Phone = row.Get<string>("phone"),
                    Star = row.Get<int>("star"),
                    Site = row.Get<string>("site"),
                    Image = Encoding.UTF8.GetBytes(row.Get<string>("image"))
                };
                requests.Add(r);
            }
            switch (sortBy)
            {
                case HotelSortBy.None:
                    break;
                case HotelSortBy.Star:
                    requests = requests.OrderBy(x => x.Star).ToList();
                    break;
                default:
                    break;
            }
            if (requests != null && desc)
            {
                requests.Reverse();
            }
            return requests;
        }

        public Hotel GetById(int id)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectById<ApiRequest>(id);
            if (table == null)
            {
                return null;
            }

            foreach (DataRow row in table.Rows)
            {
                Hotel r = new Hotel
                {
                    Id = row.Get<int>("id"),
                    Address = row.Get<string>("address"),
                    Phone = row.Get<string>("phone"),
                    Star = row.Get<int>("star"),
                    Site = row.Get<string>("site"),
                    Image = Encoding.UTF8.GetBytes(row.Get<string>("image"))
                };
                return r;
            }
            return null;
        }

        public Hotel Update(int id, Hotel obj)
        {
            if (!IsValidPhone(obj.Phone) || !IsValidUrl(obj.Site))
            {
                return null;
            }
            Hotel old = GetById(id);
            if (old != null)
            {
                string forRequest = "";
                if (old.Address != obj.Address)
                {
                    forRequest += $"address = '{obj.Address}', ";
                }
                if (old.Phone != obj.Phone)
                {
                    forRequest += $"phone = '{obj.Phone}', ";
                }
                if (old.Star != obj.Star)
                {
                    forRequest += $"star = '{obj.Star}', ";
                }
                if (old.Site != obj.Site)
                {
                    forRequest += $"site = '{obj.Site}', ";
                }
                if (old.Image != obj.Image)
                {
                    forRequest += $"image = '{Encoding.UTF8.GetString(obj.Image)}', ";
                }

                if (forRequest != "")
                {
                    forRequest = "SET " + forRequest;
                    forRequest = forRequest.Substring(0, forRequest.Length - 2);
                    forRequest += $" WHERE id = '{id}'";
                }

                var res = MySql.ExecuteNonQuery("UPDATE hotel.hotel " + forRequest);
                if (res.HasError)
                {
                    return null;
                }
                else
                {
                    return GetById(id);
                }
            }
            return null;
        }
    }
}
