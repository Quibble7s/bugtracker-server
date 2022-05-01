using System.ComponentModel.DataAnnotations;

namespace bugtracker.DTOS {
	public class CreateProjectDTO {
		[Required]
		[MinLength(1)]
		[MaxLength(64)]
		public string Name { get; init; }
		[Required]
		[MinLength(1)]
		[MaxLength(1024)]
		public string Description { get; init; }
	}
}
