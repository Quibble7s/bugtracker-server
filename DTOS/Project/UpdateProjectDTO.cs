using System.ComponentModel.DataAnnotations;

namespace bugtracker.DTOS {
	public record UpdateProjectDTO {
		[MaxLength(64)]
		public string Name { get; init; }
		[MaxLength(1024)]
		public string Description { get; init; }
	}
}
