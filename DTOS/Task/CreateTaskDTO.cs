using System.ComponentModel.DataAnnotations;

namespace bugtracker.DTOS {
	public record CreateTaskDTO {
		[Required]
		[MinLength(1)]
		[MaxLength(1024)]
		public string Description { get; init; }
	}
}
