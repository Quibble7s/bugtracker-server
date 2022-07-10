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
	[Route("api/v1/project")]
	public class ProjectController : ControllerBase {

		private readonly IUserRepo userRepo;
		private readonly IProjectRepo projectRepo;
		private readonly ILogRepo logRepo;
		private readonly IJwtUtils jwtUtils;

		public ProjectController(IUserRepo userRepo, IProjectRepo projectRepo, ILogRepo logRepo, IJwtUtils jwtUtils) {
			this.userRepo = userRepo;
			this.projectRepo = projectRepo;
			this.logRepo = logRepo;
			this.jwtUtils = jwtUtils;
		}

		private bool UserIsProjectAdmin (string userId, Project project) {
			return project.Members.Where((member) => member.Role == ProjectRole.Admin && member.User.Id == new Guid(userId)).Count() > 0;
		}

		private string GetUserId() {
			var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			var userId = jwtUtils.GetUserIdFromToken(token);
			return userId;
		}

		//GET /api/v1/project/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<ProjectDTO>> GetProjectAsync(Guid id) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));

			if (user == null)
				return NotFound(new {
					Message = $"User '{userId}' not found.",
					Status = 404
				});
			if (!user.Projects.Contains(id))
				return NotFound(new {
					Message = $"User '{user.Id}' is not part of '{id}' project.",
					Status = 404
				});

			Project project = await projectRepo.GetProjectAsync(id);
			if (project == null)
				return NotFound(new {
					Message = $"Project '{id}' not found.",
					Status = 404
				});

			return Ok(project.AsDTO());
		}

		//GET /api/v1/project/all
		[HttpGet("all")]
		public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjectsAsync() {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));

			if (user == null)
				return NotFound(new {
					Message = $"User '{userId}' not found.",
					Status = 404
				});

			return Ok((await projectRepo.GetProjectsAsync(user.Projects)).Select((project) => project.AsDTO()));
		}

		//POST /api/v1/project/create
		[HttpPost("create")]
		public async Task<ActionResult<ProjectDTO>> CreateProjectAsync(CreateProjectDTO project) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));

			if (user == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});

			DateTimeOffset createdTime = DateTimeOffset.UtcNow;

			Project createdProject = new Project {
				Id = Guid.NewGuid(),
				Name = project.Name,
				Description = project.Description,
				Members = new List<Member>() { new Member { Id = user.Id, Role = ProjectRole.Admin } },
				Bugs = new List<Bug>(),
				CreatedAt = createdTime,
				EditedAt = createdTime
			};

			await projectRepo.CreateProjectAsync(user, createdProject);
			await logRepo.SetLogAsync(createdProject.Id, $"{user.UserName} created this project.");

			return CreatedAtAction(nameof(GetProjectAsync), new { 
				id = createdProject.Id 
			},createdProject.AsDTO() with {
					Members = new List<Member>() { 
						new Member { 
							Id = user.Id, 
							User = user.AsProjectUserDTO(), 
							Role = ProjectRole.Admin 
						} 
					} 
				}
			);
		}

		//PUT /api/v1/project/{id}
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateProjectAsync(Guid id, UpdateProjectDTO updateProject) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(id);

			if (user == null)
				return NotFound(new {
					Message = $"User '{userId}' not found.",
					Status = 404
				});
			if (project == null)
				return NotFound(new {
					Message = $"Project '{id}' not found.",
					Status = 404
				});
			if (!user.Projects.Contains(project.Id))
				return NotFound(new {
					Message = $"User '{userId}' is not part of '{id}' project.",
					Status = 404
				});

			Project updatedProject = project with {
				Name = string.IsNullOrEmpty(updateProject.Name) ? project.Name : updateProject.Name,
				Description = string.IsNullOrEmpty(updateProject.Description) ? project.Description : updateProject.Description,
				EditedAt = DateTimeOffset.UtcNow
			};

			await projectRepo.UpdateProjectAsync(updatedProject);
			await logRepo.SetLogAsync(id, $"{user.UserName} updated the project.");
			return NoContent();
		}

		//PUT /api/v1/project/{id}/join
		[HttpPut("{id}/join")]
		public async Task<ActionResult> JoinProjectAsync(Guid id) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(id);

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
			if (user.Projects.Contains(project.Id))
				return NotFound(new {
					Message = $"You're already part of this project.",
					Status = 404
				});

			await projectRepo.JoinProjectAsync(user, project);
			await logRepo.SetLogAsync(id, $"{user.UserName} joined the project.");
			return NoContent();
		}

		//PUT /api/v1/project/{id}/leave
		[HttpPut("{id}/leave")]
		public async Task<ActionResult> LeaveProjectAsync(Guid id) {
			string userId = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(userId));
			Project project = await projectRepo.GetProjectAsync(id);

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

			//Delete the project and make all the other members leave if user is admin
			if(UserIsProjectAdmin(userId, project)) {
				foreach (Member member in project.Members) {
					User userMember = await userRepo.GetUserAsync(member.User.Id);
					await projectRepo.LeaveProjectAsync(userMember, project);
				}
				await projectRepo.DeleteProjectAsync(project.Id);
				return NoContent();
			}

			await projectRepo.LeaveProjectAsync(user, project);
			await logRepo.SetLogAsync(id, $"{user.UserName} left the project.");
			return NoContent();
		}

		//DELETE /api/v1/project/{id}/delete
		[HttpDelete("{id}/delete")]
		public async Task<ActionResult> DeleteProjectAsync(Guid id) {
			string userId = GetUserId();
			Project project = await projectRepo.GetProjectAsync(id);

			if (project == null)
				return NotFound(new {
					Message = $"User not found.",
					Status = 404
				});
			if (!UserIsProjectAdmin(userId, project))
				return NotFound(new {
					Message = $"User is not part of project or is not an administrator.",
					Status = 404
				});

			foreach (Member member in project.Members) {
				User userMember = await userRepo.GetUserAsync(member.User.Id);
				await projectRepo.LeaveProjectAsync(userMember, project);
			}

			await projectRepo.DeleteProjectAsync(project.Id);
			await logRepo.DeleteLogAsync(project.Id);
			return NoContent();
		}
	}
}
