using API.DAL;
using Contract.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DAL
{
    public class CheckProxy : BaseProxy
    {
        public CheckProxy(string token) : base(token)
        { }

        private static string GetBasePath(int? userID = null)
        {
            return $"check/{userID?.ToString() ?? ""}";
        }

        public async Task<IEnumerable<Check>> List()
        {
            try
            {
                string response = await client.GetStringAsync("list");
                return GetInstance<IEnumerable<Check>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Check> Add(Check model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync("Add", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Check>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Check> GetById(int id)
        {
            try
            {
                string response = await client.GetStringAsync(GetBasePath(id));
                return GetInstance<Check>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Check> Edit(int id, Check model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync($"edit/{id}", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Check>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
