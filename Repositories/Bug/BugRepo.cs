using bugtracker.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Repositories {
	public class BugRepo : IBugRepo {

		private readonly IMongoCollection<Project> projectCollection;
		private readonly FilterDefinitionBuilder<Project> projectFilter = Builders<Project>.Filter;

		public BugRepo(IMongoClient client) {
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			projectCollection = database.GetCollection<Project>(DBNames.DB_PROJECTS);
		}

		public async Task CreateBugAsync(Project project, Bug bug) {
			//Inserting the created bug into the current bug list
			List<Bug> bugs = project.Bugs.ToList();
			bugs.Add(bug);

			//Updating the project with the new list of bugs
			Project updatedProject = project with { Bugs = bugs };
			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), updatedProject);
		}

		public async Task UpdateBugAsync(Project project, Bug bug) {
			List<Bug> bugs = project.Bugs.ToList();
			Bug existingBug = bugs.Where(b => b.Id == bug.Id).SingleOrDefault();
			bugs[bugs.IndexOf(existingBug)] = bug;
			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), project with {
				Bugs = bugs,
				EditedAt = DateTimeOffset.UtcNow
			});
		}

		public async Task<Bug> GetBugAsync(Project project, Guid bugId) {
			Bug bug = project.Bugs.Where(b => b.Id == bugId).SingleOrDefault();
			await Task.CompletedTask;
			return bug;
		}

		public async Task DeleteBugAsync(Project project, Guid bugId) {
			List<Bug> bugs = project.Bugs.ToList();
			Bug bugToRemove = bugs.Where(bug => bug.Id == bugId).SingleOrDefault();
			if (bugToRemove != null) {
				bugs.RemoveAt(bugs.IndexOf(bugToRemove));
				await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), project with {
					Bugs = bugs
				});
			}
		}
	}
}
