using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bugtracker.Lib;
using bugtracker.Models;
using MongoDB.Driver;

namespace bugtracker.Repositories {
	public class ProjectRepo : IProjectRepo {

		private readonly IMongoCollection<User> userCollection;
		private readonly IMongoCollection<Project> projectCollection;
		private readonly FilterDefinitionBuilder<User> userFilter = Builders<User>.Filter;
		private readonly FilterDefinitionBuilder<Project> projectFilter = Builders<Project>.Filter;

		public ProjectRepo(IMongoClient client) {
			IMongoDatabase database = client.GetDatabase(DBNames.DB_NAME);
			userCollection = database.GetCollection<User>(DBNames.DB_USERS);
			projectCollection = database.GetCollection<Project>(DBNames.DB_PROJECTS);
		}

		public async Task CreateProjectAsync(User user, Project project) {
			await projectCollection.InsertOneAsync(project);

			List<Guid> updatedUserProjectList = (List<Guid>)user.Projects;
			updatedUserProjectList.Add(project.Id);
			User updatedUser = user with { Projects = updatedUserProjectList };

			await userCollection.ReplaceOneAsync(userFilter.Eq(user => user.Id, user.Id), updatedUser);
		}

		public async Task DeleteProjectAsync(Guid id) {
			await projectCollection.DeleteOneAsync(projectFilter.Eq(project => project.Id, id));
		}

		private async Task<IEnumerable<Member>> GetProjectUsersMembers(List<Member> members) {
			List<Member> output = new List<Member>();
			for (int i = 0; i < members.Count(); i++) {
				User user = await userCollection.Find(userFilter.Eq(user => user.Id, members[i].Id)).SingleOrDefaultAsync();
				if (user != null)
					output.Add(members[i] with { User = user.AsProjectUserDTO()});
			}
			return output;
		}

		public async Task<Project> GetProjectAsync(Guid id) {
			Project existingProject = await projectCollection.Find(projectFilter.Eq((project) => project.Id, id)).SingleOrDefaultAsync();
			if(existingProject != null) {
				Project project = existingProject with {
					Members = await GetProjectUsersMembers(existingProject.Members.ToList())
				};
				return project;
			}
			return null;
		}

		public async Task<IEnumerable<Project>> GetProjectsAsync(IEnumerable<Guid> ids) {
			List<Project> projects = new List<Project>();
			foreach (Guid id in ids) {
				Project project = await GetProjectAsync(id);
				if (project != null)
					projects.Add(project);
			}
			return projects;
		}

		public async Task JoinProjectAsync(User user, Project project) {
			//Putting the project id in the user project list
			List<Guid> projects = user.Projects.ToList();
			projects.Add(project.Id);
			User updatedUser = user with { Projects = projects, EditedAt = DateTimeOffset.UtcNow };
			await userCollection.ReplaceOneAsync(userFilter.Eq(u => u.Id, user.Id), updatedUser);

			//Putting the new member in the project member list
			List<Member> currentMembers = project.Members.ToList();
			currentMembers.Add(new Member { Id = user.Id, Role = ProjectRole.Member });
			Project updatedProject = project with { Members = currentMembers, EditedAt = DateTimeOffset.UtcNow };
			await projectCollection.ReplaceOneAsync(projectFilter.Eq(p => p.Id, project.Id), updatedProject);
		}

		public async Task LeaveProjectAsync(User user, Project project) {
			//Removing the project id from the user project list
			List<Guid> userProjectList = user.Projects.ToList();
			int projectIndex = userProjectList.IndexOf(project.Id);
			userProjectList.RemoveAt(projectIndex);
			User updatedUser = user with { Projects = userProjectList, EditedAt = DateTimeOffset.UtcNow };
			await userCollection.ReplaceOneAsync(userFilter.Eq(existingUser => existingUser.Id, user.Id), updatedUser);

			//Removing the member from the project members list
			List<Member> projectMembers = project.Members.ToList();
			Member memberToRemove = projectMembers.Where((member) => member.Id == user.Id).SingleOrDefault();
			int memberIndex = projectMembers.IndexOf(memberToRemove);
			projectMembers.RemoveAt(memberIndex);
			Project updatedProject = project with { Members = projectMembers, EditedAt = DateTimeOffset.UtcNow };
			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), updatedProject);
		}

		public async Task UpdateProjectAsync(Project project) {
			await projectCollection.ReplaceOneAsync(projectFilter.Eq(prj => prj.Id, project.Id), project);
		}
	}
}
