using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Contract.Consts
{
    public static class JwtManager
    {
        public static bool ValidateToken(string token, out ClaimsPrincipal user)
        {
            user = GetPrincipal(token);
            if (!(user?.Identity is ClaimsIdentity identity))
            {
                return false;
            }
            if (!identity.IsAuthenticated)
            {
                return false;
            }
            Claim idClaim = identity.FindFirst(TokenClaims.ID);
            if (idClaim == null || !int.TryParse(idClaim.Value, out int id))
            {
                return false;
            }
            return true;
        }

        private static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!(tokenHandler.ReadToken(token) is JwtSecurityToken jwtToken))
                {
                    return null;
                }

                //byte[] symmetricKey = Convert.FromBase64String(Secret);
                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateLifetime = true,
                    ValidateAudience = false,

                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("Pax7YTNcCnW0YcmUsPG4NKxIunK4aPC5yZYLhNdQGY4/KN+pQSnMzonUR5uLzVXycvI5DKWFGHePXbq0TKaIRg==")),
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}