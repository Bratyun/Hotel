using API.Interfaces;
using Contract.Models;
using MySqlLib;
using System;
using System.Collections.Generic;
using System.Data;

namespace API.DAL
{
    public class ApiCheck : ApiGeneral, IWorkWithDB<Check>
    {
        public ApiCheck(MySqlData mySql) : base(mySql)
        {
        }

        public Check Create(Check obj)
        {
            if (!(obj.RegisterDate.Date > DateTime.Now.Date))
            {
                return null;
            }
            ApiUser userManager = new ApiUser(MySql);
            ApiRoom roomManager = new ApiRoom(MySql);
            User user = userManager.GetById(obj.UserId);
            Room room = roomManager.GetById(obj.RoomId);
            if (user == null || room == null)
            {
                return null;
            }
            string request = $"INSERT INTO hotel.check (userId, roomId, isDeleted, registerDate, status) VALUES (" +
                $"'{obj.UserId}', '{obj.RoomId}', '{obj.IsDeleted}', '{obj.RegisterDate}', '{obj.Status}')";
            var res = MySql.ExecuteNonQuery(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                var toReturn = MySql.ExecuteReader($"SELECT * FROM hotel.check WHERE userId = '{obj.UserId}' " +
                    $"AND roomId = '{obj.RoomId}' AND isDeleted = '{obj.IsDeleted}' AND registerDate = '{obj.RegisterDate}' AND status = '{obj.Status}'");
                if (toReturn.HasError)
                {
                    return null;
                }
                else
                {
                    foreach (DataRow row in toReturn.Result.Rows)
                    {
                        Check r = new Check
                        {
                            Id = row.Get<int>("id"),
                            UserId = row.Get<int>("userId"),
                            RoomId = row.Get<int>("roomId"),
                            IsDeleted = row.Get<bool>("isDeleted"),
                            RegisterDate = row.Get<DateTime>("registerDate"),
                            Status = (CheckStatus)row.Get<int>("status"),
                        };
                        return r;
                    }
                    return null;
                }
            }
        }

        public List<Check> GetAllByUser(int id)
        {
            List<Check> all = GetAll();
            List<Check> results = new List<Check>();
            foreach (var item in all)
            {
                if (item.UserId == id)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public bool IsFreeRoom(DateTime start, DateTime end, int roomId)
        {
            var toReturn = MySql.ExecuteReader($"SELECT * FROM hotel.check WHERE dateStart <= '{end}' AND dateEnd >= '{start}' AND roomId = '{roomId}'");
            if (toReturn.HasError)
            {
                return false;
            }
            else
            {
                return toReturn.Result.Rows.Count == 0;
            }
        }

        public bool Delete(int id)
        {
            Requester requester = new Requester(MySql);
            return requester.Remove<ApiCheck>(id);
        }

        public List<Check> GetAll()
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectAll<ApiCheck>();
            if (table == null)
            {
                return null;
            }

            List<Check> checks = new List<Check>();
            foreach (DataRow row in table.Rows)
            {
                Check r = new Check
                {
                    Id = row.Get<int>("id"),
                    UserId = row.Get<int>("userId"),
                    RoomId = row.Get<int>("roomId"),
                    IsDeleted = row.Get<bool>("isDeleted"),
                    RegisterDate = row.Get<DateTime>("registerDate"),
                    Status = (CheckStatus)row.Get<int>("status"),
                };
                checks.Add(r);
            }
            return checks;
        }

        public Check GetById(int id)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectById<ApiCheck>(id);
            if (table == null)
            {
                return null;
            }

            foreach (DataRow row in table.Rows)
            {
                Check r = new Check
                {
                    Id = row.Get<int>("id"),
                    UserId = row.Get<int>("userId"),
                    RoomId = row.Get<int>("roomId"),
                    IsDeleted = row.Get<bool>("isDeleted"),
                    RegisterDate = row.Get<DateTime>("registerDate"),
                    Status = (CheckStatus)row.Get<int>("status"),
                };
                return r;
            }
            return null;
        }

        public Check Update(int id, Check obj)
        {
            if (!(obj.RegisterDate.Date > DateTime.Now.Date))
            {
                return null;
            }
            ApiUser userManager = new ApiUser(MySql);
            ApiRoom roomManager = new ApiRoom(MySql);
            User user = userManager.GetById(obj.UserId);
            Room room = roomManager.GetById(obj.RoomId);
            if (user == null || room == null)
            {
                return null;
            }
            Check old = GetById(id);
            if (old != null)
            {
                string forRequest = "";
                if (old.UserId != obj.UserId)
                {
                    forRequest += $"userId = '{obj.UserId}', ";
                }
                if (old.RoomId != obj.RoomId)
                {
                    forRequest += $"roomId = '{obj.RoomId}', ";
                }
                if (old.IsDeleted != obj.IsDeleted)
                {
                    forRequest += $"isDeleted = '{obj.IsDeleted}', ";
                }
                if (old.RegisterDate != obj.RegisterDate)
                {
                    forRequest += $"registerDate = '{obj.RegisterDate}', ";
                }
                if (old.Status != obj.Status)
                {
                    forRequest += $"status = '{obj.Status}', ";
                }

                if (forRequest != "")
                {
                    forRequest = "SET " + forRequest;
                    forRequest = forRequest.Substring(0, forRequest.Length - 2);
                    forRequest += $" WHERE id = '{id}'";
                }

                var res = MySql.ExecuteNonQuery("UPDATE hotel.check " + forRequest);
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
