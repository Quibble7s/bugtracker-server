using bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Repositories {
	public interface ILogRepo {
		Task<Log> GetLogAsync(Guid id);
		Task<IEnumerable<LogMessage>> GetLogMessagesAsync(Guid id);
		Task SetLogAsync(Guid id, string message);
		Task DeleteLogAsync(Guid id);
	}
}
