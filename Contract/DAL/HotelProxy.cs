using API.DAL;
using Contract.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DAL
{
    public class HotelProxy : BaseProxy
    {
        public HotelProxy(string token) : base(token)
        { }

        private static string GetBasePath(int? userID = null)
        {
            return $"hotel/{userID?.ToString() ?? ""}";
        }

        public async Task<IEnumerable<Hotel>> List(HotelSortBy sortBy = HotelSortBy.None, bool desc = false)
        {
            try
            {
                string response = await client.GetStringAsync($"list/{sortBy}/{desc}");
                return GetInstance<IEnumerable<Hotel>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Hotel> Add(Hotel model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync("Add", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Hotel>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Hotel> GetById(int id)
        {
            try
            {
                string response = await client.GetStringAsync(GetBasePath(id));
                return GetInstance<Hotel>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Hotel> Edit(int id, Hotel model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync($"edit/{id}", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Hotel>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(GetBasePath(id));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
