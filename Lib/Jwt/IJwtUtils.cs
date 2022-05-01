using bugtracker.Models;

namespace bugtracker.Lib.Jwt {
	public interface IJwtUtils {
		public string GenerateToken(User user);
		public string GetUserIdFromToken(string token);
	}
}
