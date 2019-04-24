using API.DAL;
using Contract.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DAL
{
    public class RoomProxy : BaseProxy
    {
        public RoomProxy(string token) : base(token)
        { }

        private static string GetBasePath(int? userID = null)
        {
            return $"room/{userID?.ToString() ?? ""}";
        }

        public async Task<IEnumerable<Room>> List(RoomSortBy orderBy = RoomSortBy.None, bool desc = false)
        {
            try
            {
                string response = await client.GetStringAsync($"list/{orderBy}/{desc}");
                return GetInstance<IEnumerable<Room>>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Room> Add(Room model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync("Add", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Room>(response.ToString());
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Room> GetById(int id)
        {
            try
            {
                string response = await client.GetStringAsync(GetBasePath(id));
                return GetInstance<Room>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Room> GetByHotel(int id, RoomSortBy orderBy = RoomSortBy.None, bool desc = false)
        {
            try
            {
                string response = await client.GetStringAsync($"byHotel/{id}/{orderBy}/{desc}");
                return GetInstance<Room>(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Room> Edit(int id, Room model)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync($"edit/{id}", GetContent(model));
                if (response.IsSuccessStatusCode)
                    return GetInstance<Room>(response.ToString());
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
                HttpResponseMessage response = await client.GetAsync($"delete/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
