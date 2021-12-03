using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT_101
{
    public class App
    {
        const string mySecret = "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";

        public App()
        {
            _args = new string[0];
        }

        public App(string[] args)
        {
            _args = args;
        }

        private string[] _args;

        public string GenerateToken(int userId)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim("nombre", "jose"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateCurrentToken(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }

        public void run()
        {
            var jwtToken = GenerateToken(123);
            Console.WriteLine(jwtToken);
            //var jwtToken = "ya29.a0ARrdaM_s8i9kSd7Ck92T8zvJHufYPXC7mybVObrRs7YUtmKmd9_lfeYT3UWOJibKStDhMVh0N9bNSazcgQuCcTVcNzEK0mPRactMx2MODFTiKK125JfBThgiIxurFKi3mLGos2TF7nm2jsuseAgi-chVfuFKVA";

            //var isValid = ValidateCurrentToken(jwtToken);
            
            //Console.WriteLine($"Is valid: {isValid}");

            //Console.WriteLine($"{ClaimTypes.NameIdentifier}: {GetClaim(jwtToken, ClaimTypes.NameIdentifier)}");
            //Console.WriteLine($"nombre: {GetClaim(jwtToken, "nombre")}");
        }
    }
}
