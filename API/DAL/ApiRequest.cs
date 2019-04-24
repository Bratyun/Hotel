using System;
using System.Collections.Generic;
using System.Data;
using API.Interfaces;
using Contract.Models;
using MySqlLib;

namespace API.DAL
{
    public class ApiRequest : ApiGeneral, IWorkWithDB<Request>
    {
        public ApiRequest(MySqlData mySql) : base(mySql)
        {
        }

        public Request Create(Request obj)
        {
            string request = $"INSERT INTO hotel.request (size, comfort, startDate, endDate, userId, status, answer) VALUES (" +
                $"'{obj.RoomSize}', '{obj.StartDate}', '{obj.EndDate}', '{obj.UserId}', '{obj.Status}', '{obj.Answer}')";
            var res = MySql.ExecuteNonQuery(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                var toReturn = MySql.ExecuteReader($"SELECT * FROM hotel.request WHERE size = '{obj.RoomSize}' AND comfort = '{obj.Comfort}' AND startDate = '{obj.StartDate}' " +
                    $"AND endDate = '{obj.EndDate}' AND userId = '{obj.UserId}' AND status = '{obj.Status}' AND answer = '{obj.Answer}'");
                if (toReturn.HasError)
                {
                    return null;
                }
                else
                {
                    foreach (DataRow row in toReturn.Result.Rows)
                    {
                        Request r = new Request
                        {
                            Id = row.Get<int>("id"),
                            RoomSize = row.Get<int>("size"),
                            Comfort = row.Get<int>("comfort"),
                            StartDate = row.Get<DateTime>("startDate"),
                            EndDate = row.Get<DateTime>("endDate"),
                            UserId = row.Get<int>("userId"),
                            Status = (RequestStatus)row.Get<int>("status"),
                            Answer = row.Get<int>("answer"),
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
            return requester.Remove<ApiRequest>(id);
        }

        public List<Request> GetAll()
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectAll<ApiRequest>();
            if (table == null)
            {
                return null;
            }

            List<Request> requests = new List<Request>();
            foreach (DataRow row in table.Rows)
            {
                Request r = new Request
                {
                    Id = row.Get<int>("id"),
                    RoomSize = row.Get<int>("size"),
                    Comfort = row.Get<int>("comfort"),
                    StartDate = row.Get<DateTime>("startDate"),
                    EndDate = row.Get<DateTime>("endDate"),
                    UserId = row.Get<int>("userId"),
                    Status = (RequestStatus)row.Get<int>("status"),
                    Answer = row.Get<int>("answer"),
                };
                requests.Add(r);
            }
            return requests;
        }

        public Request GetById(int id)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectById<ApiRequest>(id);
            if (table == null)
            {
                return null;
            }

            foreach (DataRow row in table.Rows)
            {
                Request r = new Request
                {
                    Id = row.Get<int>("id"),
                    RoomSize = row.Get<int>("size"),
                    Comfort = row.Get<int>("comfort"),
                    StartDate = row.Get<DateTime>("startDate"),
                    EndDate = row.Get<DateTime>("endDate"),
                    UserId = row.Get<int>("userId"),
                    Status = (RequestStatus)row.Get<int>("status"),
                    Answer = row.Get<int>("answer"),
                };
                return r;
            }
            return null;
        }

        public Request Update(int id, Request obj)
        {
            Request old = GetById(id);
            if (old != null)
            {
                string forRequest = "";
                if (old.RoomSize != obj.RoomSize)
                {
                    forRequest += $"size = '{obj.RoomSize}', ";
                }
                if (old.Comfort != obj.Comfort)
                {
                    forRequest += $"comfort = '{obj.Comfort}', ";
                }
                if (old.StartDate != obj.StartDate)
                {
                    forRequest += $"startDate = '{obj.StartDate}', ";
                }
                if (old.EndDate != obj.EndDate)
                {
                    forRequest += $"endDate = '{obj.EndDate}', ";
                }
                if (old.UserId != obj.UserId)
                {
                    forRequest += $"userId = '{obj.UserId}', ";
                }
                if (old.Status != obj.Status)
                {
                    forRequest += $"status = '{obj.Status}', ";
                }
                if (old.Answer != obj.Answer)
                {
                    forRequest += $"answer = '{obj.Answer}', ";
                }

                if (forRequest != "")
                {
                    forRequest = "SET " + forRequest;
                    forRequest = forRequest.Substring(0, forRequest.Length - 2);
                    forRequest += $" WHERE id = '{id}'";
                }

                var res = MySql.ExecuteNonQuery("UPDATE hotel.request " + forRequest);
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
