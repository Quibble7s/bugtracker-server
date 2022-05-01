using bugtracker.Models;
using System.Threading.Tasks;

namespace bugtracker.Repositories {
	public interface IAuthRepo {
		Task<object> LoginAsync(string email, string password);
		Task<User> RegisterAsync(User user);
	}
}
