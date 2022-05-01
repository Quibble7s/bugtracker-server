namespace bugtracker.DTOS {
	public record UpdateUserDTO {
		public string UserName { get; init; }
		public string Email { get; init; }
		public string Password { get; init; }
		public string ProfilePictureUrl { get; init; }
	}
}
