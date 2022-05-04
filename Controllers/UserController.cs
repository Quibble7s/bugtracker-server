using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bugtracker.DTOS;
using bugtracker.Lib;
using bugtracker.Repositories;
using bugtracker.Models;
using Microsoft.AspNetCore.Authorization;
using bugtracker.Lib.Jwt;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace bugtracker.Controllers {

	[Authorize]
	[ApiController]
	[Route("api/v1/user")]
	public class UserController : ControllerBase {

		private readonly IUserRepo userRepo;
		private readonly IProjectRepo projectRepo;
		private readonly IJwtUtils jwtUtils;

		public UserController(IUserRepo userRepo, IProjectRepo projectRepo, IJwtUtils jwtUtils) {
			this.userRepo = userRepo;
			this.projectRepo = projectRepo;
			this.jwtUtils = jwtUtils;
		}
		private string GetUserId() {
			var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
			var userId = jwtUtils.GetUserIdFromToken(token);
			return userId;
		}

		//GET /api/v1/user
		[Authorize(Roles = UserRole.Admin)]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersAsync() {
			return Ok((await userRepo.GetUsersAsync()).Select(user => user.AsDTO()));
		}

		//GET /api/v1/user/me
		[HttpGet("me")]
		public async Task<ActionResult<UserDTO>> GetUserAsync() {
			string id = GetUserId();
			User user = await userRepo.GetUserAsync(new Guid(id));
			if (user == null) {
				return NotFound(new {
					Message = $"User '{id}' not found.",
					Status = 404
				});
			}
			return Ok(user.AsDTO());
		}

		//GET /api/v1/user/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<UserDTO>> GetUserAsync(Guid id) {

			User user = await userRepo.GetUserAsync(id);

			if (user == null) {
				return NotFound(new { 
					Message = $"User '{id}' not found.",
					Status = 404
				});
			}

			return Ok(user.AsDTO());
		}

		//PUT /api/v1/user/{id}
		[HttpPut("update")]
		public async Task<ActionResult> UpdateUserAsync(UpdateUserDTO user) {
			Guid id = new Guid(GetUserId());

			User existingUser = await userRepo.GetUserAsync(id);

			if (existingUser == null)
				return NotFound(new {
					Message = $"User '{id}' not found.",
					Status = 404
				});

			User updatedUser = existingUser with {
				UserName = string.IsNullOrEmpty(user.UserName) ? existingUser.UserName : user.UserName,
				Email = string.IsNullOrEmpty(user.Email) ? existingUser.Email : user.Email,
				Password = string.IsNullOrEmpty(user.Password) ? existingUser.Password : user.Password,
				ProfilePictureUrl = string.IsNullOrEmpty(user.ProfilePictureUrl) ? existingUser.ProfilePictureUrl : user.ProfilePictureUrl,
				EditedAt = DateTimeOffset.UtcNow
			};

			await userRepo.UpdateUserAsync(updatedUser);
			return NoContent();
		}

		//PUT /api/v1/user/{id}
		[HttpPut("image")]
		public async Task<IActionResult> UpdateUserProfilePictureAsync([FromForm][Required] IFormFile data) {
			//Disabled
			return BadRequest(new {
				Message = $"Disabled since heroku doesn't support container volumes.",
				Status = 400
			});

			Guid id = new Guid(GetUserId());

			string[] validExtensions = new string[] {"jpg", "png", "svg", "gif"};

			if (!validExtensions.Any(ext => data.FileName.EndsWith(ext))) {
				return BadRequest(new {
					Message = $"File extension is invalid.",
					Status = 400
				});
			}

			User user = await userRepo.GetUserAsync(id);

			if (user == null)
				return NotFound(new {
					Message = $"User '{id}' not found.",
					Status = 404
				});

			bool success = await userRepo.UpdateUserProfilePictureAsync(data, user);

			if (!success)
				return BadRequest(new {
					Message = $"An error ocurred while trying to upload '{data.FileName}'",
					Status = 400
				});

			return NoContent();
		}

		//DELETE /api/v1/user/{id}/delete
		[HttpDelete("delete")]
		public async Task<ActionResult> DeleteUserAsync() {
			Guid id = new Guid(GetUserId());

			User user = await userRepo.GetUserAsync(id);

			if (user == null)
				return NotFound(new {
					Message = $"User '{id}' not found.",
					Status = 404
				});

			//Finding all the projects where the user is not the admin.
			List<Project> projectsMember = (await projectRepo.GetProjectsAsync(user.Projects)).Where(project => {
				foreach (Member member in project.Members) {
					if (member.Id == user.Id && member.Role != ProjectRole.Admin)
						return true;
				}
				return false;
			}).ToList();

			//Leaving all the projects where the user is not the admin.
			foreach (Project project in projectsMember) {
				await projectRepo.LeaveProjectAsync(user, project);
			}

			//Finding all the projects where the user is the admin.
			List<Project> projectsAdmin = (await projectRepo.GetProjectsAsync(user.Projects)).Where(project => {
				foreach (Member member in project.Members) {
					if (member.Id == user.Id && member.Role == ProjectRole.Admin)
						return true;
				}
				return false;
			}).ToList();
			
			//Deleting and leaving all the projects where the user is the admin.
			for (int i = 0; i < projectsAdmin.Count; i++) {
				List<Member> members = projectsAdmin[i].Members.ToList();
				for (int j = 0; j < members.Count; j++) {
					await projectRepo.LeaveProjectAsync(await userRepo.GetUserAsync(members[j].User.Id), projectsAdmin[i]);
				}
				await projectRepo.DeleteProjectAsync(projectsAdmin[i].Id);
			}

			await userRepo.DeleteUserAsync(id);
			return NoContent();
		}
	}
}
