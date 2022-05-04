using BCryptNet = BCrypt.Net.BCrypt;
using bugtracker.DTOS;
using bugtracker.Models;
using bugtracker.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using bugtracker.Lib;

namespace bugtracker.Controllers {
	[Authorize]
	[ApiController]
	[Route("api/v1/auth")]
	public class AuthController : ControllerBase {

		private readonly IAuthRepo authRepo;
		private readonly IUserRepo userRepo;

		public AuthController(IAuthRepo authRepo, IUserRepo userRepo) {
			this.authRepo = authRepo;
			this.userRepo = userRepo;
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult<object>> LoginAsync(LoginDTO login) {
			var auth = await authRepo.LoginAsync(login.Email, login.Password);
			if (auth == null)
				return Unauthorized(new{
					Message = "Invalid email or password.", 
					Status = 401 
				});
			return Ok(auth);
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult<object>> RegisterAsync(CreateUserDTO createUser) {
			DateTimeOffset date = DateTimeOffset.UtcNow;

			User user = new User {
				Id = Guid.NewGuid(),
				UserName = createUser.UserName,
				Email = createUser.Email,
				Password = BCryptNet.HashPassword(createUser.Password),
				Role = UserRole.User,
				CreatedAt = date,
				EditedAt = date,
				Projects = new List<Guid>()
			};

			//This will return null if the user already exist.
			User created = await authRepo.RegisterAsync(user);
			if(created == null) {
				return BadRequest(new { 
					Message = $"User '{createUser.Email}' already exist.",
					Status = 400
				});
			}

			var loggedUser = await authRepo.LoginAsync(createUser.Email, createUser.Password);
			return CreatedAtAction(nameof(LoginAsync), loggedUser);
		}
	}
}
