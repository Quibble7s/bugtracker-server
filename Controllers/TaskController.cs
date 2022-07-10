using bugtracker.DTOS;
using bugtracker.Lib;
using bugtracker.Lib.Jwt;
using bugtracker.Models;
using bugtracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Controllers {
	[Authorize]
	[ApiController]
	[Route("api/v1/task")]
	public class TaskController : ControllerBase {

		private readonly IUserRepo userRepo;
		private readonly IProjectRepo projectRepo;
		private readonly IBugRepo bugRepo;
		private readonly ITaskRepo taskRepo;
		private readonly ILogRepo logRepo;
		private readonly IJwtUtils jwtUtils;

		public TaskController(IUserRepo userRepo, IProjectRepo projectRepo, IBugRepo bugRepo, ITaskRepo taskRepo, ILogRepo logRepo, IJwtUtils jwtUtils) {
			this.userRepo = userRepo;
			this.projectRepo = projectRepo;
			this.bugRepo = bugRepo;
			this.taskRepo = taskRepo;
			this.logRepo = logRepo;
			this.jwtUtils = jwtUtils;
		}

		private bool UserIsProjectAdmin(string userId, Project project) {
			return project.Members.Where((member) => member.Role == ProjectRole.Admin && member.User.Id == new Guid(userId)).Count() > 0;
		}

		private string GetUserId() {
			var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			var userId = jwtUtils.GetUserIdFromToken(token);
			return userId;
		}



		//GET /api/v1/task/{id}/project/{projectId}/bug/{bugId}/user/{userId}
		[HttpGet("{id}/project/{projectId}/bug/{bugId}")]
		public async Task<ActionResult<TaskDTO>> GetTaskAsync(Guid id, Guid projectId, Guid bugId) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project not found.",
					Status = 404
				});
			if (!user.Projects.Contains(project.Id))
				return NotFound(new {
					Message = $"User is not part of project.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, bugId);
			if (bug == null)
				return NotFound(new {
					Message = $"Bug not found.",
					Status = 404
				});

			return Ok(await taskRepo.GetTaskAsync(bug, id));
		}

		//POST /api/v1/task/project/{projectId}/bug/{bugId}/user/{userId}/create
		[HttpPost("project/{projectId}/bug/{bugId}")]
		public async Task<ActionResult<TaskDTO>> CreateTaskAsync(Guid projectId, Guid bugId, CreateTaskDTO createTask) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project not found.",
					Status = 404
				});
			if (!UserIsProjectAdmin(userId, project))
				return NotFound(new {
					Message = $"User is not part of project or is not an administrator.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, bugId);
			if (bug == null)
				return NotFound(new {
					Message = $"Bug not found.",
					Status = 404
				});

			BugTask createdTask = new BugTask { 
				Id = Guid.NewGuid(),
				Description = createTask.Description,
				State = TaskState.Pending
			};

			await taskRepo.CreateTaskAsync(project, bug, createdTask);
			await logRepo.SetLogAsync(bug.Id, $"{user.UserName} created \"{createdTask.Description}\" task.");
			return CreatedAtAction(nameof(GetTaskAsync), new { 
				id = createdTask.Id, 
				projectId = project.Id, 
				bugId = bug.Id
			}, createdTask.AsDTO());
		}

		//PUT /api/v1/task/{id}/project/{projectId}/bug/{bugId}/user/{userId}
		[HttpPut("{id}/project/{projectId}/bug/{bugId}/state")]
		public async Task<ActionResult> UpdateTaskState(Guid id, Guid projectId, Guid bugId, UpdateTaskDTO updateTask) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project not found.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, bugId);
			if (bug == null)
				return NotFound(new {
					Message = $"Issue not found.",
					Status = 404
				});
			BugTask task = await taskRepo.GetTaskAsync(bug, id);
			if (task == null)
				return NotFound(new {
					Message = $"Task not found.",
					Status = 404
				});

			BugTask updatedTask = task with {
				State = updateTask.State == task.State || string.IsNullOrEmpty(updateTask.State) ? task.State : updateTask.State
			};

			await taskRepo.UpdateTaskAsync(project, bug, updatedTask);
			await logRepo.SetLogAsync(bug.Id, $"{user.UserName} updated the state of \"{task.Description}\" to \"{updatedTask.State}\".");
			return NoContent();
		}

		//PUT /api/v1/task/{id}/project/{projectId}/bug/{bugId}/user/{userId}
		[HttpPut("{id}/project/{projectId}/bug/{bugId}/description")]
		public async Task<ActionResult> UpdateTaskDescription(Guid id, Guid projectId, Guid bugId, UpdateTaskDTO updateTask) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project not found.",
					Status = 404
				});
			if (!UserIsProjectAdmin(userId, project))
				return NotFound(new {
					Message = $"User is not part of project or is not an administrator.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, bugId);
			if (bug == null)
				return NotFound(new {
					Message = $"Issue not found.",
					Status = 404
				});
			BugTask task = await taskRepo.GetTaskAsync(bug, id); 
			if (task == null)
				return NotFound(new {
					Message = $"Task not found.",
					Status = 404
				});

			BugTask updatedTask = task with {
				Description = string.IsNullOrEmpty(updateTask.Description) ? task.Description : updateTask.Description
			};

			await taskRepo.UpdateTaskAsync(project, bug, updatedTask);
			await logRepo.SetLogAsync(bug.Id, $"{user.UserName} updated task description from \"{task.Description}\" to \"{updatedTask.Description}\".");
			return NoContent();
		}

		//DELETE /api/v1/task/{id}/project/{projectId}/bug/{bugId}/user/{userId}/delete
		[HttpDelete("{id}/project/{projectId}/bug/{bugId}/delete")]
		public async Task<ActionResult> DeleteTaskAsync(Guid id, Guid projectId, Guid bugId) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project not found.",
					Status = 404
				});
			if (!UserIsProjectAdmin(userId, project))
				return NotFound(new {
					Message = $"User is not part of project or is not an administrator.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, bugId);
			if (bug == null)
				return NotFound(new {
					Message = $"Bug not found.",
					Status = 404
				});
			BugTask task = await taskRepo.GetTaskAsync(bug, id);
			if (task == null)
				return NotFound(new {
					Message = $"Task not found.",
					Status = 404
				});

			await taskRepo.DeleteTaskAsync(project, bug, id);
			await logRepo.SetLogAsync(bug.Id, $"{user.UserName} removed \"{task.Description}\" task.");
			return NoContent();
		}
	}
}
