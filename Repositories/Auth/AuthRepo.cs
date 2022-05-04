using bugtracker.Config;
using bugtracker.Lib;
using bugtracker.Lib.Jwt;
using bugtracker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace bugtracker.Repositories {
	public class AuthRepo : IAuthRepo {

		private readonly IJwtUtils jwtUtils;
		private readonly IConfiguration configuration;
		private readonly IMongoCollection<User> userCollection;
		private readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;

		public AuthRepo(IMongoClient client, IConfiguration configuration, IJwtUtils jwtUtils) {
			this.jwtUtils = jwtUtils;
			this.configuration = configuration;
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			userCollection = database.GetCollection<User>(DBNames.DB_USERS);
		}

		public async Task<object> LoginAsync(string email, string password) {
			User user = await userCollection.Find(u => u.Email.ToLower() == email.ToLower()).SingleOrDefaultAsync();
			bool verified = BCryptNet.Verify(password, user?.Password);

			if (user == null || !verified)
				return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.ASCII.GetBytes(configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>().Secret);

			var tokenDescriptor = new SecurityTokenDescriptor() { 
				Subject = new ClaimsIdentity(new Claim[] { 
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role),
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = jwtUtils.GenerateToken(user);

			return new { User = user.AsDTO(), Token = token };
		}

		public async Task<User> RegisterAsync(User user) {
			User existingUser = await userCollection.Find(userFilter.Eq(u => u.Email, user.Email)).SingleOrDefaultAsync();
			if (existingUser != null)
				return null;
			await userCollection.InsertOneAsync(user);
			return user;
		}
	}
}
