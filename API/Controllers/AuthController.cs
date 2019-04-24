using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DAL;
using Contract.Consts;
using Contract.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ApiUser users;

        public AuthController(ApiUser users)
        {
            this.users = users;
        }

        [HttpPost("Login")]
        public ActionResult<string> Login([FromBody] LoginRequestModel model)
        {
            ClaimsIdentity identity = GetIdentity(model.Login, AuthProxy.GetPasswordHash(model.Password));
            if (identity == null)
            {
                return BadRequest("Invalid login or password.");
            }

            var now = DateTime.UtcNow;
            byte[] symmetricKey = Convert.FromBase64String(AuthOptionsPrivate.KEY);
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(AuthOptions.LIFETIME),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        private ClaimsIdentity GetIdentity(string login, string passwordHash)
        {
            User user = users.GetByLogin(login);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(TokenClaims.ID, user.Id.ToString()),
                    new Claim(TokenClaims.Name, user.FirstName),
                    new Claim(TokenClaims.Role, user.Role.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Token", TokenClaims.Name, TokenClaims.Role);
                return claimsIdentity;
            }
            return null;
        }
    }
}