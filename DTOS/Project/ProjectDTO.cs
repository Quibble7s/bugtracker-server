using bugtracker.Models;
using System;
using System.Collections.Generic;

namespace bugtracker.DTOS {
	public record ProjectDTO {
		public Guid Id { get; init; }
		public string Name { get; init; }
		public string Description { get; init; }
		public IEnumerable<Member> Members { get; init; }
		public IEnumerable<Bug> Bugs { get; init; }
		public DateTimeOffset CreatedAt { get; init; }
		public DateTimeOffset EditedAt { get; init; }
	}
}
