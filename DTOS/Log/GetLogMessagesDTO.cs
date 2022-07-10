using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.DTOS {
	public record GetLogMessagesDTO {
		[Required]
		[Range(1, int.MaxValue)]
		public int Page { get; init; }
		[Required]
		[Range(10, 50)]
		public int PerPage { get; init; }
	}
}
