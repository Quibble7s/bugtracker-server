using System;
using System.Threading.Tasks;
using bugtracker.Models;

namespace bugtracker.Repositories {
	public interface ITaskRepo {
		Task CreateTaskAsync(Project project, Bug bug, BugTask task);
		Task<BugTask> GetTaskAsync(Bug bug, Guid id);
		Task UpdateTaskAsync(Project project, Bug bug, BugTask task);
		Task DeleteTaskAsync(Project project, Bug bug, Guid id);
	}
}
