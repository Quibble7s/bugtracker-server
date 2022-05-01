using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bugtracker.Models;
using Microsoft.AspNetCore.Http;

namespace bugtracker.Repositories
{
  public interface IUserRepo{
    Task<User> GetUserAsync(Guid id);
    Task<IEnumerable<User>> GetUsersAsync();
    Task UpdateUserAsync(User user);
    Task<bool> UpdateUserProfilePictureAsync(IFormFile file, User user);
    Task DeleteUserAsync(Guid id);
  }
}