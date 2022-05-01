using bugtracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Repositories {
	public class TaskRepo : ITaskRepo {

		private readonly IMongoCollection<Project> projectCollection;
		private readonly FilterDefinitionBuilder<Project> projectFilter = Builders<Project>.Filter;

		public TaskRepo(IMongoClient client) {
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			projectCollection = database.GetCollection<Project>(DBNames.DB_PROJECTS);
		}

		public async Task CreateTaskAsync(Project project, Bug bug, BugTask task) {
			List<Bug> bugs = project.Bugs.ToList();
			int bugIndex = bugs.IndexOf(bug);

			List<BugTask> tasks = bug.Tasks.ToList();
			tasks.Add(task);

			bug = bug with { Tasks = tasks };
			bugs[bugIndex] = bug;

			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), project with {
				Bugs = bugs,
				EditedAt = DateTimeOffset.UtcNow
			});
		}

		public async Task DeleteTaskAsync(Project project, Bug bug, Guid id) {
			int bugIndex = project.Bugs.ToList().IndexOf(bug);

			BugTask task = await GetTaskAsync(bug, id);
			int taskIndex = bug.Tasks.ToList().IndexOf(task);

			List<BugTask> updatedTasks = bug.Tasks.ToList();
			updatedTasks.RemoveAt(taskIndex);

			bug = bug with { Tasks = updatedTasks };

			List<Bug> updatedBugs = project.Bugs.ToList();
			updatedBugs[bugIndex] = bug;

			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), project with {
				Bugs = updatedBugs,
				EditedAt = DateTimeOffset.UtcNow
			});
		}

		public async Task<BugTask> GetTaskAsync(Bug bug, Guid id) {
			BugTask task = bug.Tasks.Where(t => t.Id == id).SingleOrDefault();
			await Task.CompletedTask;
			return task;
		}

		public async Task UpdateTaskAsync(Project project, Bug bug, BugTask task) {
			//Getting the current list of bugs and getting the index of the bug where we want to update a task.
			List<Bug> updatedBugs = project.Bugs.ToList();
			int bugIndex = updatedBugs.IndexOf(bug);

			//Getting the current list of task and replacing the existing task with the updated one.
			List<BugTask> updatedTasks = bug.Tasks.ToList();
			int taskIndex = updatedTasks.IndexOf(bug.Tasks.Where(t => t.Id == task.Id).SingleOrDefault());
			updatedTasks[taskIndex] = task;

			//Replacing the current list of tasks with the updated one.
			bug = bug with {
				Tasks = updatedTasks
			};
			//Replacing the current bug with the updated one.
			updatedBugs[bugIndex] = bug;

			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), project with {
				Bugs = updatedBugs,
				EditedAt = DateTimeOffset.UtcNow
			});
		}
	}
}
