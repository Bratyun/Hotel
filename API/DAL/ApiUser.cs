using API.Interfaces;
using Contract.Models;
using MySqlLib;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace API.DAL
{
    public class ApiUser : ApiGeneral, IWorkWithDB<User>
    {
        public ApiUser(MySqlData mySql) : base(mySql)
        {
        }

        public User Create(User user)
        {
            if (!IsValidEmail(user.Email) || !IsValidPhone(user.Phone))
            {
                return null;
            }
            string request = $"INSERT INTO hotel.user (login, password, firstName, email, phone, role) VALUES ('{user.Login}', " +
                $"'{user.Password}', '{user.FirstName}', '{user.Email}', '{user.Phone}', '{user.Role}')";
            var res = MySql.ExecuteNonQuery(request);
            if (res.HasError)
            {
                return null;
            }
            else
            {
                return GetByLogin(user.Login);
            }
        }

        public bool Delete(int id)
        {
            Requester requester = new Requester(MySql);
            return requester.Remove<ApiUser>(id);
        }

        public List<User> GetAll()
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectAll<ApiUser>();

            List<User> users = new List<User>();
            foreach (DataRow row in table.Rows)
            {
                User user = new User
                {
                    Id = row.Get<int>("id"),
                    FirstName = row.Get<string>("firstName"),
                    Phone = row.Get<string>("phone"),
                    Email = row.Get<string>("email"),
                    Role = row.Get<string>("role"),
                    Login = row.Get<string>("login"),
                    Password = row.Get<string>("password")
                };
                users.Add(user);
            }
            return users;
        }

        public User GetById(int id)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectById<ApiUser>(id);

            foreach (DataRow row in table.Rows)
            {
                User user = new User
                {
                    Id = row.Get<int>("id"),
                    FirstName = row.Get<string>("firstName"),
                    Phone = row.Get<string>("phone"),
                    Email = row.Get<string>("email"),
                    Role = row.Get<string>("role"),
                    Login = row.Get<string>("login"),
                    Password = row.Get<string>("password")
                };
                return user;
            }
            return null;
        }

        public User GetByLogin(string login)
        {
            Requester requester = new Requester(MySql);
            DataTable table = requester.SelectByLogin<ApiUser>(login);

            foreach (DataRow row in table.Rows)
            {
                User user = new User
                {
                    Id = row.Get<int>("id"),
                    FirstName = row.Get<string>("firstName"),
                    Phone = row.Get<string>("phone"),
                    Email = row.Get<string>("email"),
                    Role = row.Get<string>("role"),
                    Login = row.Get<string>("login"),
                    Password = row.Get<string>("password")
                };
                return user;
            }
            return null;
        }

        public User Update(int id, User user)
        {
            if (!IsValidEmail(user.Email) || !IsValidPhone(user.Phone))
            {
                return null;
            }
            User old = GetById(id);
            if (old != null)
            {
                string forRequest = "";
                if (old.Login != user.Login)
                {
                    forRequest += $"login = '{user.Login}', ";
                }
                if (old.Password != user.Password)
                {
                    forRequest += $"password = '{user.Password}', ";
                }
                if (old.FirstName != user.FirstName)
                {
                    forRequest += $"firstName = '{user.FirstName}', ";
                }
                if (old.Email != user.Email)
                {
                    forRequest += $"email = '{user.Email}', ";
                }
                if (old.Phone != user.Phone)
                {
                    forRequest += $"phone = '{user.Phone}', ";
                }
                if (old.Role != user.Role)
                {
                    forRequest += $"Role = '{user.Role}', ";
                }

                if (forRequest != "")
                {
                    forRequest = "SET " + forRequest;
                    forRequest = forRequest.Substring(0, forRequest.Length - 2);
                    forRequest += $" WHERE id = '{id}'";
                }

                var res = MySql.ExecuteNonQuery("UPDATE hotel.user " + forRequest);
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
