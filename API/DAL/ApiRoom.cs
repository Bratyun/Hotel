using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using API.Interfaces;
using Contract.Models;
using MySqlLib;

namespace API.DAL
{
    public class ApiRoom : ApiGeneral, IWorkWithDB<Room>
    {
        public ApiRoom(MySqlData mySql) : base(mySql)
        {
        }

        public Room Create(Room obj)
        {
            string request = $"INSERT INTO hotel.room (number, hotelId, size, comfort, image, price, status, startDate, endDate, userId) VALUES (" +
                $"'{obj.Number}', '{obj.HotelId}', '{obj.RoomSize}', '{obj.Comfort}', '{Encoding.UTF8.GetString(obj.Image)}', '{obj.Price}', '{obj.Status}', " +
                $"'{obj.StartDate}', '{obj.EndDate}', '{obj.UserId}')";
            var res = MySql.ExecuteNonQuery(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                var toReturn = MySql.ExecuteReader($"SELECT * FROM hotel.room WHERE number = '{obj.Number}' AND hotelId = '{obj.HotelId}' AND size = '{obj.RoomSize}' " +
                    $"AND size = '{obj.RoomSize}' AND comfort = '{obj.Comfort}' AND image = '{Encoding.UTF8.GetString(obj.Image)}' AND price = '{obj.Price}' AND status = '{obj.Status}' AND" +
                    $"startDate = '{obj.StartDate}' AND endDate = '{obj.EndDate}' AND userId = '{obj.UserId}'");
                if (toReturn.HasError)
                {
                    return null;
                }
                else
                {
                    foreach (DataRow row in toReturn.Result.Rows)
                    {
                        Room room = new Room
                        {
                            Id = row.Get<int>("id"),
                            Number = row.Get<int>("number"),
                            HotelId = row.Get<int>("hotelId"),
                            RoomSize = row.Get<int>("size"),
                            Comfort = row.Get<int>("comfort"),
                            Image = Encoding.UTF8.GetBytes(row.Get<string>("image")),
                            Price = row.Get<double>("price"),
                            Status = (RoomStatus)row.Get<int>("status"),
                            StartDate = row.Get<DateTime>("dateStart"),
                            EndDate = row.Get<DateTime>("dateEnd"),
                            UserId = row.Get<int>("userId")
                        };
                        return room;
                    }
                    return null;
                }
            }
        }

        internal void SetRoomFree(int id)
        {
            Room room = GetById(id);
            room.UserId = 0;
            room.Status = RoomStatus.Free;
            room.StartDate = default(DateTime);
            room.EndDate = default(DateTime);
            Update(id, room);
        }

        public bool Delete(int id)
        {
            Requester requester = new Requester(MySql);
            return requester.Remove<ApiRoom>(id);
        }

        public List<Room> GetAll()
        {
            return GetAll(RoomSortBy.None, false);
        }

        public List<Room> GetAll(RoomSortBy sortBy, bool desc)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectAll<ApiRoom>();
            if (table == null)
            {
                return null;
            }

            List<Room> rooms = new List<Room>();
            foreach (DataRow row in table.Rows)
            {
                Room room = new Room
                {
                    Id = row.Get<int>("id"),
                    Number = row.Get<int>("number"),
                    HotelId = row.Get<int>("hotelId"),
                    RoomSize = row.Get<int>("size"),
                    Comfort = row.Get<int>("comfort"),
                    Image = Encoding.UTF8.GetBytes(row.Get<string>("image")),
                    Price = row.Get<double>("price"),
                    Status = (RoomStatus)row.Get<int>("status"),
                    StartDate = row.Get<DateTime>("dateStart"),
                    EndDate = row.Get<DateTime>("dateEnd"),
                    UserId = row.Get<int>("userId")
                };

                rooms.Add(room);
            }
            switch (sortBy)
            {
                case RoomSortBy.None:
                    break;
                case RoomSortBy.RoomSize:
                    rooms = rooms.OrderBy(x => x.RoomSize).ToList();
                    break;
                case RoomSortBy.Comfort:
                    rooms = rooms.OrderBy(x => x.Comfort).ToList();
                    break;
                case RoomSortBy.Status:
                    rooms = rooms.OrderBy(x => x.Status).ToList();
                    break;
                case RoomSortBy.Price:
                    rooms = rooms.OrderBy(x => x.Price).ToList();
                    break;
                default:
                    break;
            }
            if (rooms != null && desc)
            {
                rooms.Reverse();
            }
            return rooms;
        }

        public Room GetById(int id)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectById<ApiRoom>(id);
            if (table == null)
            {
                return null;
            }

            foreach (DataRow row in table.Rows)
            {
                Room room = new Room
                {
                    Id = row.Get<int>("id"),
                    Number = row.Get<int>("number"),
                    HotelId = row.Get<int>("hotelId"),
                    RoomSize = row.Get<int>("size"),
                    Comfort = row.Get<int>("comfort"),
                    Image = Encoding.UTF8.GetBytes(row.Get<string>("image")),
                    Price = row.Get<double>("price"),
                    Status = (RoomStatus)row.Get<int>("status"),
                    StartDate = row.Get<DateTime>("dateStart"),
                    EndDate = row.Get<DateTime>("dateEnd"),
                    UserId = row.Get<int>("userId")
                };
                return room;
            }
            return null;
        }

        public List<Room> GetByComfortAndSize(int comfort, int size)
        {
            string request = $"SELECT * FROM hotel.room WHERE comfort = '{comfort}' AND size = '{size}'";
            var res = MySql.ExecuteReader(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                List<Room> rooms = new List<Room>();
                foreach (DataRow row in res.Result.Rows)
                {
                    Room room = new Room
                    {
                        Id = row.Get<int>("id"),
                        Number = row.Get<int>("number"),
                        HotelId = row.Get<int>("hotelId"),
                        RoomSize = row.Get<int>("size"),
                        Comfort = row.Get<int>("comfort"),
                        Image = Encoding.UTF8.GetBytes(row.Get<string>("image")),
                        Price = row.Get<double>("price"),
                        Status = (RoomStatus)row.Get<int>("status"),
                        StartDate = row.Get<DateTime>("dateStart"),
                        EndDate = row.Get<DateTime>("dateEnd"),
                        UserId = row.Get<int>("userId")
                    };

                    rooms.Add(room);
                }
                return rooms;
            }
        }

        public Room Update(int id, Room obj)
        {
            Room old = GetById(id);
            if (old != null)
            {
                string forRequest = "";
                if (old.Number != obj.Number)
                {
                    forRequest += $"number = '{obj.Number}', ";
                }
                if (old.HotelId != obj.HotelId)
                {
                    forRequest += $"hotelId = '{obj.HotelId}', ";
                }
                if (old.RoomSize != obj.RoomSize)
                {
                    forRequest += $"size = '{obj.RoomSize}', ";
                }
                if (old.Comfort != obj.Comfort)
                {
                    forRequest += $"comfort = '{obj.Comfort}', ";
                }
                if (old.Image != obj.Image)
                {
                    forRequest += $"image = '{obj.Image}', ";
                }
                if (old.Price != obj.Price)
                {
                    forRequest += $"price = '{obj.Price}', ";
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

                var res = MySql.ExecuteNonQuery("UPDATE hotel.room " + forRequest);
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
