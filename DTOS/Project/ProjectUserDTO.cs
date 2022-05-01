using System;

namespace bugtracker.DTOS {
	public record ProjectUserDTO {
		public Guid Id { get; init; }
		public string UserName { get; init; }
		public string ProfilePictureUrl { get; init; }
	}
}
