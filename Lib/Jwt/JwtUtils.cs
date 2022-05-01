using bugtracker.Config;
using bugtracker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bugtracker.Lib.Jwt {
	public class JwtUtils : IJwtUtils {

		private readonly IConfiguration configuration;

		public JwtUtils(IConfiguration configuration) {
			this.configuration = configuration;
		}

		public string GenerateToken(User user) {
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.ASCII.GetBytes(configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>().Secret);

			var tokenDescriptor = new SecurityTokenDescriptor() {
				Subject = new ClaimsIdentity(new Claim[] {
					new Claim("id", user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role),
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public string GetUserIdFromToken(string token) {
      if (token == null)
        return null;

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>().Secret);
      try {
        tokenHandler.ValidateToken(token, new TokenValidationParameters {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

        return userId;
      }
      catch {
        return null;
      }
    }
	}
}
