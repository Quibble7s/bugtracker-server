using bugtracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Repositories {
	public class LogRepo : ILogRepo {

		private readonly IMongoCollection<Log> logCollection;
		private readonly FilterDefinitionBuilder<Log> logFilter = Builders<Log>.Filter;

		public LogRepo(IMongoClient client) {
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			logCollection = database.GetCollection<Log>(DBNames.DB_PROJECT_LOGS);
		}

		private int Clamp(int min, int max, int value) {
			return value < min ? min : value > max ? max : value;
		}

		public async Task DeleteLogAsync(Guid id) {
			await logCollection.DeleteOneAsync(logFilter.Eq((log) => log.Id, id));
		}

		public async Task<Log> GetLogAsync(Guid id) {
			return await logCollection.Find(logFilter.Eq((log) => log.Id, id)).SingleOrDefaultAsync();
		}

		public async Task<IEnumerable<LogMessage>> GetLogMessagesAsync(Guid id) {
			Log log = await GetLogAsync(id);

			if(log != null) {
				return log.Messages;
			}
			return null;
		}

		public async Task SetLogAsync(Guid id, string message) {
			Log currentLog = await GetLogAsync(id);
			//If the log exist just update it with the new message.
			if(currentLog != null) {
				Log updatedLog = currentLog with { 
					Messages = currentLog.Messages.Append(new LogMessage { 
						Id = Guid.NewGuid(), Message = message, Date = DateTimeOffset.UtcNow 
					}) 
				};
				await logCollection.ReplaceOneAsync(logFilter.Eq((log) => log.Id, id), updatedLog);
				return;
			}

			//If the log doesn't exist yet create a new one with the message.
			Log newLog = new Log { 
				Id = id, Messages = new List<LogMessage>() { 
					new LogMessage {
						Id= new Guid(), Message= message, Date= DateTimeOffset.UtcNow 
					} 
				} 
			};
			await logCollection.InsertOneAsync(newLog);
		}
	}
}
