using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bugtracker.Models;

namespace bugtracker.Repositories {
	public interface IProjectRepo {
    Task CreateProjectAsync(User user, Project project);
    Task<Project> GetProjectAsync(Guid id);
    Task<IEnumerable<Project>> GetProjectsAsync(IEnumerable<Guid> ids);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Guid id);
    Task JoinProjectAsync(User user, Project project);
    Task LeaveProjectAsync(User user, Project project);
  }
}
