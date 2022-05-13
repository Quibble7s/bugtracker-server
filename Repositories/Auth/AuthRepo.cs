using bugtracker.Lib;
using bugtracker.Lib.Jwt;
using bugtracker.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace bugtracker.Repositories {
	public class AuthRepo : IAuthRepo {

		private readonly IJwtUtils jwtUtils;
		private readonly IMongoCollection<User> userCollection;
		private readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;

		public AuthRepo(IMongoClient client, IJwtUtils jwtUtils) {
			this.jwtUtils = jwtUtils;
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			userCollection = database.GetCollection<User>(DBNames.DB_USERS);
		}

		public async Task<object> LoginAsync(string email, string password) {
			User user = await userCollection.Find(u => u.Email.ToLower() == email.ToLower()).SingleOrDefaultAsync();
			if (user == null)
				return null;

			bool verified = BCryptNet.Verify(password, user.Password);
			if (!verified)
				return null;

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
