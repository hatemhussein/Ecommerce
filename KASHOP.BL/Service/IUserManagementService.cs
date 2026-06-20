using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BL.Service
{
    public interface IUserManagementService
    {
        Task<List<UserListResponse>> GetAllUsers();
        Task<UserDetailsResponse?> GetUser(string userId);
        Task<bool> ChangeRole(string userId, string role);
        Task<bool> toggleBlockUser(string userId);
        Task<bool> DeleteUser(string userId);

    }
}
