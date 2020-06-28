using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Authenticate;
using EStore.API.Service.Models.User;
using System;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface IUserService : ICommonService
    {
        Task<EnEntityExistsStatus> UserExistsAsync(Guid idUser);
        Task<EnEntityExistsStatus> UserExistsAsync(string username);
        Task<Output<UserModel>> GetUserAsync(Guid idUser);
        Task<Output<UserModel>> CreateUserAsync(UserInsertModel userInsertModel, string password);
        Task<Output<AuthenticatedModel>> AuthenticateUserAsync(AuthenticateModel authenticateModel);
        Task<Output<dynamic>> DeleteUserAsync(UserDeleteModel userDeleteModel);
    }
}
