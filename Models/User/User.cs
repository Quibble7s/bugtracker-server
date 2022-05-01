using System;
using System.Collections.Generic;

namespace bugtracker.Models {
	public record User{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
    public string Email { get; init; } 
    public string ProfilePictureUrl { get; init; }
    public string Role {get; init;}
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset EditedAt { get; init; }
    public IEnumerable<Guid> Projects { get; init; }
  }
}