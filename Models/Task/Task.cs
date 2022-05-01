using System;

namespace bugtracker.Models
{
    public record BugTask{
      public Guid Id { get; init; }
      public string Description { get; init; }
      public string State { get; init; }
    }
}