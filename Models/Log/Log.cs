using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bugtracker.Models {
	public record Log {
		public Guid Id { get; init; }
		public IEnumerable<LogMessage> Messages { get; init; }
	}
}
