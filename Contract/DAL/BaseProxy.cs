using Contract.Consts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL
{
    public class BaseProxy
    {
        protected readonly HttpClient client;

        public BaseProxy(string token = null)
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(Urls.API)
            };
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected StringContent GetContent<T>(T instance)
        {
            string json = JsonConvert.SerializeObject(instance);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

        protected async Task<T> GetInstance<T>(HttpContent response)
            => GetInstance<T>(await response.ReadAsStringAsync());

        protected T GetInstance<T>(string responseString)
        {
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(responseString, typeof(T));
            }

            T instance = JsonConvert.DeserializeObject<T>(responseString);
            return instance;
        }
    }
}
