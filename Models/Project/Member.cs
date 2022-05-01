using bugtracker.DTOS;
using System;

namespace bugtracker.Models {
	public record Member{
		public Guid Id { get; init; }
		public ProjectUserDTO User { get; init; }
    public string Role { get; init; }
  }
}