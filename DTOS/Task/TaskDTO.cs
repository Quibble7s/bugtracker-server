using System;

namespace bugtracker.DTOS {
	public record TaskDTO {
		public Guid Id { get; init; }
		public string Description { get; init; }
		public string State { get; init; }
	}
}
