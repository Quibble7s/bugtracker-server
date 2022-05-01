using bugtracker.DTOS;
using System;
using System.Collections.Generic;

namespace bugtracker.Models {
	public record Bug {
    public Guid Id { get; init; }
		public string Name { get; init; }
		public string Description { get; init; }
    public string Priority { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IEnumerable<BugTask> Tasks { get; init; }
  }
}