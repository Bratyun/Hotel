using Contract.Consts;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL
{
    public class AuthProxy : BaseProxy
    {

        #region static
        public static string GetPasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = SHA256(bytes);
            string hash = Convert.ToBase64String(hashedBytes);
            return hash;
        }

        protected static byte[] SHA256(byte[] bytes)
        {
            var crypt = new SHA256Managed();
            byte[] hash = crypt.ComputeHash(bytes);
            return hash;
        }
        #endregion

        public async Task<string> Login(LoginRequestModel model)
        {
            HttpResponseMessage response = await client.PostAsync($"auth", GetContent(model));
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await GetInstance<string>(response.Content);
        }
    }
}
