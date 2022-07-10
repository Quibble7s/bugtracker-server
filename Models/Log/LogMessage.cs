using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Models {
	public record LogMessage {
		public Guid Id { get; init; }
		public string Message { get; init; }
		public DateTimeOffset Date { get; init; }
	}
}
