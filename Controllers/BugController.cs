using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using bugtracker.DTOS;
using bugtracker.Lib;
using bugtracker.Lib.Jwt;
using bugtracker.Models;
using bugtracker.Repositories;

namespace bugtracker.Controllers {
	[Authorize]
	[ApiController]
	[Route("api/v1/bug")]
	public class BugController : ControllerBase {

		private readonly IUserRepo userRepo;
		private readonly IProjectRepo projectRepo;
		private readonly IBugRepo bugRepo;
		private readonly ILogRepo logRepo;
		private readonly IJwtUtils jwtUtils;

		public BugController(IUserRepo userRepo, IProjectRepo projectRepo, IBugRepo bugRepo, ILogRepo logRepo, IJwtUtils jwtUtils) {
			this.userRepo = userRepo;
			this.projectRepo = projectRepo;
			this.bugRepo = bugRepo;
			this.logRepo = logRepo;
			this.jwtUtils = jwtUtils;
		}

		private string GetUserId() {
			var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			var userId = jwtUtils.GetUserIdFromToken(token);
			return userId;
		}

		//GET /api/v1/bug/{id}/project/{projectId}
		[HttpGet("{id}/project/{projectId}")]
		public async Task<ActionResult<BugDTO>> GetBugAsync(Guid id, Guid projectId) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User '{userId}' not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project '{projectId}' not found.",
					Status = 404
				});
			if (!user.Projects.Contains(project.Id))
				return NotFound(new {
					Message = $"User '{userId}' is not part of '{projectId}' project.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, id);
			if (bug == null)
				return NotFound(new {
					Message = $"Bug '{id}' not found.",
					Status = 404
				});
			return Ok(bug.AsDTO());
		}

		//PUT /api/v1/bug/project/{projectId}/create
		[HttpPost("project/{projectId}/create")]
		public async Task<ActionResult<BugDTO>> CreateBugAsync(Guid projectId, CreateBugDTO bug) {
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
			if (project.Members.Where((member) => member.Role == ProjectRole.Admin && member.User.Id == new Guid(userId)).Count() < 1)
				return NotFound(new {
					Message = $"User is not part of project or is not an administrator.",
					Status = 404
				});

			DateTimeOffset createdTime = DateTimeOffset.UtcNow;
			Bug createdBug = new Bug {
				Id = Guid.NewGuid(),
				Name = bug.Name,
				Description = bug.Description,
				Priority = bug.Priority,
				Tasks = new List<BugTask>(),
				CreatedAt = createdTime,
			};

			await bugRepo.CreateBugAsync(project, createdBug);
			await logRepo.SetLogAsync(project.Id, $"{user.UserName} created \"{createdBug.Name}\" issue.");
			await logRepo.SetLogAsync(createdBug.Id, $"{user.UserName} created this issue.");
			return CreatedAtAction(nameof(GetBugAsync), new {
				id = createdBug.Id,
				projectId = project.Id
			}, createdBug.AsDTO());
		}

		//PUT /api/v1/bug/{id}/project/{projectId}
		[HttpPut("{id}/project/{projectId}")]
		public async Task<ActionResult> UpdateBugAsync(Guid id, Guid projectId, UpdateBugDTO updateBug) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User '{userId}' not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project '{projectId}' not found.",
					Status = 404
				});
			if (project.Members.Where((member) => member.Role == ProjectRole.Admin && member.User.Id == new Guid(userId)).Count() < 1)
				return NotFound(new {
					Message = $"User '{userId}' is not part of '{projectId}' project or is not an administrator.",
					Status = 404
				});

			Bug bug = await bugRepo.GetBugAsync(project, id);
			if (bug == null)
				return NotFound(new {
					Message = $"Bug '{id}' not found.",
					Status = 404
				});

			await bugRepo.UpdateBugAsync(project, bug with {
				Name = string.IsNullOrEmpty(updateBug.Name) ? bug.Name : updateBug.Name,
				Description = string.IsNullOrEmpty(updateBug.Description) ? bug.Description : updateBug.Description,
				Priority = updateBug.Priority == bug.Priority || string.IsNullOrEmpty(updateBug.Priority) ? bug.Priority : updateBug.Priority
			});
			await logRepo.SetLogAsync(bug.Id, $"{user.UserName} updated this issue.");
			return NoContent();
		}

		//DELETE /api/v1/bug/{id}/project/{projectId}/delete
		[HttpDelete("{id}/project/{projectId}/delete")]
		public async Task<ActionResult> DeleteBugAsync(Guid id, Guid projectId) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(projectId);

			if (user == null)
				return NotFound(new {
					Message = $"User '{userId}' not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project '{projectId}' not found.",
					Status = 404
				});
			if (project.Members.Where((member) => member.Role == ProjectRole.Admin && member.User.Id == new Guid(userId)).Count() < 1)
				return NotFound(new {
					Message = $"User '{userId}' is not part of '{projectId}' project or is not an administrator.",
					Status = 404
				});

			await bugRepo.DeleteBugAsync(project, id);
			await logRepo.DeleteLogAsync(id);
			return NoContent();
		}
	}
}
