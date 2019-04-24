using Contract.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.DAL
{
    public class UsersProxy : BaseProxy
    {
        public UsersProxy(string token) : base(token)
        { }
        
        private static string GetBasePath(int? userID = null)
        {
            return $"user/{userID?.ToString() ?? "current"}";
        }

        public async Task<IEnumerable<User>> GetList()
        {
            try
            {
                string response = await client.GetStringAsync("user");
                return GetInstance<IEnumerable<User>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public User Convert(User us)
        {
            return new User()
            {
                FirstName = us.FirstName,
                Phone = us.Phone,
                Email = us.Email,
                Role = "User",
                Login = us.Login,
                Password = us.Password
            
            };

        }

        public async Task<User> Get(int? id = null)
        {
            try
            {
                string response = await client.GetStringAsync(GetBasePath(id));
                return GetInstance<User>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Add(User user)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync("user", GetContent(user));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
}

        public async Task<bool> Update(User user, int? id = null)
        {
            try
            {
                HttpResponseMessage response = await client.PutAsync(GetBasePath(id), GetContent(user));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(GetBasePath(id));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}