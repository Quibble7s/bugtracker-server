using bugtracker.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace bugtracker.Repositories
{
  public class UserRepo : IUserRepo{

		private readonly IMongoCollection<User> userCollection;
		private readonly IWebHostEnvironment env;
		private readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;

		public UserRepo(IMongoClient client, IWebHostEnvironment env) {
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			userCollection = database.GetCollection<User>(DBNames.DB_USERS);
			this.env = env;
		}

		public async Task DeleteUserAsync(Guid id) {
			await userCollection.DeleteOneAsync(userFilter.Eq(user => user.Id, id));
		}

		public async Task<User> GetUserAsync(Guid id) {
			return await userCollection.Find(userFilter.Eq((user) => user.Id, id)).SingleOrDefaultAsync();
		}

		public async Task<IEnumerable<User>> GetUsersAsync() {
			return await userCollection.Find(new BsonDocument()).ToListAsync();
		}

		public async Task UpdateUserAsync(User user) {
			await userCollection.ReplaceOneAsync(userFilter.Eq(existingUser => existingUser.Id, user.Id), user);
		}

		public async Task<bool> UpdateUserProfilePictureAsync(IFormFile file, User user) {
			try {
				string fileName = $"{Guid.NewGuid()}-{file.FileName}";
				string path = Path.Combine(env.ContentRootPath, "static", "user", user.Id.ToString(), "images");
				string fullPath = Path.Combine(path, fileName);

				//Create directory if doesn't exist
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				await file.CopyToAsync(new FileStream(fullPath, FileMode.Create));
				await UpdateUserAsync(user with { ProfilePictureUrl = $"static/user/{user.Id}/images/{fileName}" });
				return true;
			}
			catch {
				return false;
			}
		}
	}
}