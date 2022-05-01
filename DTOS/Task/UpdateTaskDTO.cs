using bugtracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.DTOS {
	public record UpdateTaskDTO {
		[MaxLength(1024)]
		public string Description { get; init; }
		public string State { get; init; }
	}
}
