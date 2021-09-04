using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using efishingAPI.JsonModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace efishingAPI.helpers
{
    public class Jwt
    {
        private readonly JsonAppSettings _appSettings;

        public Jwt(IOptions<JsonAppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string generate(int id)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credentials);

            var payload = new JwtPayload(id.ToString(), null, null, null, expires: DateTime.Today.AddDays(1));
            var securityToken = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public JwtSecurityToken verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedTocken);

            return (JwtSecurityToken)validatedTocken;
        }
    }
}
