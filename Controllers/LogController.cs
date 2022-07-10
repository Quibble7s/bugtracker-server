using bugtracker.DTOS;
using bugtracker.Lib.Jwt;
using bugtracker.Models;
using bugtracker.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bugtracker.Controllers {
	[Authorize]
	[ApiController]
	[Route("api/v1/log")]
	public class LogController : ControllerBase {

		private readonly ILogRepo logRepo;

		public LogController(ILogRepo logRepo) {
			this.logRepo = logRepo;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Log>> GetLog(Guid id) {
			Log log = await logRepo.GetLogAsync(id);
			if(log == null) {
				return NotFound(new { 
					Message = "Log not found.",
					Status = 404
				});
			}
			return Ok(log);
		}

		[HttpGet("{id}/messages")]
		public async Task<ActionResult<IEnumerable<LogMessage>>> GetMessages(Guid id) {
			List<LogMessage> messages = (List<LogMessage>)await logRepo.GetLogMessagesAsync(id);
			if(messages == null) {
				return Ok(new { total = 0, messages = new List<LogMessage>() });
			}
			messages.Reverse();
			return Ok(new { total = messages.Count, messages = messages});
		}
	}
}
