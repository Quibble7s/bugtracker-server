using bugtracker.Models;
using System;
using System.Collections.Generic;

namespace bugtracker.DTOS {
	public record BugDTO {
    public Guid Id { get; init; }
		public string Name { get; init; }
		public string Description { get; init; }
    public string Priority { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IEnumerable<BugTask> Tasks { get; init; }
  }
}
