using bugtracker.DTOS;
using bugtracker.Models;

namespace bugtracker.Lib {
	public static class Extensions {

		public static UserDTO AsDTO(this User user) => new UserDTO {
			Id = user.Id,
			UserName = user.UserName,
			Email = user.Email,
			CreatedAt = user.CreatedAt,
			EditedAt = user.EditedAt,
			ProfilePictureUrl = user.ProfilePictureUrl,
			Role = user.Role,
			Projects = user.Projects
		};

		public static BugDTO AsDTO(this Bug bug) => new BugDTO {
			Id = bug.Id,
			Name = bug.Name,
			Description = bug.Description,
			Priority = bug.Priority,
			Tasks = bug.Tasks,
			CreatedAt = bug.CreatedAt
		};

		public static ProjectDTO AsDTO(this Project project) => new ProjectDTO {
			Id = project.Id,
			Name = project.Name,
			Description = project.Description,
			Members = project.Members,
			Bugs = project.Bugs,
			CreatedAt = project.CreatedAt,
			EditedAt = project.EditedAt
		};

		public static TaskDTO AsDTO(this BugTask task) => new TaskDTO { 
			Id = task.Id,
			Description = task.Description,
			State = task.State
		};

		public static ProjectUserDTO AsProjectUserDTO(this User user) => new ProjectUserDTO {
			Id = user.Id,
			ProfilePictureUrl = user.ProfilePictureUrl,
			UserName = user.UserName
		};

	}
}
