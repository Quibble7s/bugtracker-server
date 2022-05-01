using bugtracker.Models;
using System;
using System.Threading.Tasks;

namespace bugtracker.Repositories {
	public interface IBugRepo {
    Task CreateBugAsync(Project project, Bug bug);
    Task<Bug> GetBugAsync(Project project, Guid bugId);
    Task DeleteBugAsync(Project project, Guid bugId);
    Task UpdateBugAsync(Project project, Bug bug);
  }
}
