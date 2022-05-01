using System.ComponentModel.DataAnnotations;

namespace bugtracker.DTOS {
	public record CreateUserDTO {
		[Required]
		[MinLength(1)]
		[MaxLength(32)]
		public string UserName { get; init; }

		[Required]
		[EmailAddress]
		public string Email { get; init; }

		[Required]
		[MinLength(8)]
		public string Password { get; init; }

	}
}
