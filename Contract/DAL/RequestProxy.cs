using API.DAL;
using Contract.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DAL
{
    public class RequestProxy : BaseProxy
    {
        public RequestProxy(string token) : base(token)
        { }

        private static string GetBasePath(int? userID = null)
        {
            return $"request/{userID?.ToString() ?? ""}";
        }
        
        public async Task<IEnumerable<Request>> List()
        {
            try
            {
                string response = await client.GetStringAsync("list");
                return GetInstance<IEnumerable<Request>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<Request> Add(Request model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync("Add", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Request>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<Request> GetById(int id)
        {
            try
            {
                string response = await client.GetStringAsync(GetBasePath(id));
                return GetInstance<Request>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public async Task<IEnumerable<Request>> GetByStatus(RequestStatus status)
        {
            try
            {
                string response = await client.GetStringAsync($"byStatus/{status}");
                return GetInstance<IEnumerable<Request>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        
        public async Task<Request> Edit(int id, Request model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync($"edit/{id}", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Request>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
