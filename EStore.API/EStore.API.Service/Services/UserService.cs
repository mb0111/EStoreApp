using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Authenticate;
using EStore.API.Service.Models.Configuration;
using EStore.API.Service.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace EStore.API.Service.Services
{
    public class UserService : IUserService
    {
        #region Private Property

        private readonly string ServiceName = nameof(UserService);
        private readonly EStoreContext _dbContext;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        #endregion

        #region Constructor
        public UserService(EStoreContext dbContext,
                           IOptions<JwtConfiguration> options,
                           IMapper mapper,
                           ILogger<UserService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _jwtConfiguration = options.Value ?? throw new ArgumentNullException(nameof(JwtConfiguration));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Common Service

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _logger.LogInformation($"{ServiceName}. Adding an object of type {entity.GetType().Name} to context");
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _logger.LogInformation($"{ServiceName}. Removing an object of type {entity.GetType().Name} from context");
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            _logger.LogInformation($"{ServiceName}. Attempting to save the changes in the context");
            try
            {
                return (await _dbContext.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"{ServiceName} error while SaveChanges");
                return false;
            }
        }

        #endregion

        #region Service Implementation

        public async Task<EnEntityExistsStatus> UserExistsAsync(Guid idUser)
        {
            _logger.LogInformation($"Checking if {nameof(User)} with IdUser: {idUser} exists");
            if (idUser.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while checking if {nameof(User)} exists. {nameof(User.IdUser)} cannot be empty Guid");
                return EnEntityExistsStatus.BadRequest;
            }
            User user = await FindUserAsync(idUser);
            return user == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        public async Task<EnEntityExistsStatus> UserExistsAsync(string username)
        {
            _logger.LogInformation($"Checking if {nameof(User)} with Username: {username} exists");
            if (username.IsNullOrWhiteSpace())
            {
                _logger.LogError($"Validation error while checking if {nameof(User)} exists. {nameof(User.UserName)} cannot be empty or whitespace only string.");
                return EnEntityExistsStatus.BadRequest;
            }
            User user = await FindUserByUsernameAsync(username);
            return user == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        public async Task<Output<UserModel>> GetUserAsync(Guid idUser)
        {
            _logger.LogInformation($"Searching for {typeof(User).Name} with IdUser: {idUser}");
            if (idUser.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while searching for {typeof(User).Name} with IdUser {Guid.Empty}");
                return new Output<UserModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdUser {idUser} is not valid"
                };
            }
            User user = await FindUserAsync(idUser);
            if (user != null)
            {
                return new Output<UserModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<UserModel>(user)
                };
            }
            _logger.LogInformation($"No {typeof(User).Name} found with IdUser {idUser}");
            return new Output<UserModel>()
            {
                Status = EnResultStatus.BadRequest,
                Message = $"Get could not be completed. Please try again later"
            };
        }

        public async Task<Output<UserModel>> CreateUserAsync(UserInsertModel userInsertModel, string password)
        {
            if (password.IsNullOrWhiteSpace())
            {
                _logger.LogWarning($"Password {password} is not valid");
                return new Output<UserModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Password is not valid"
                };
            }
            if (await UserExistsAsync(userInsertModel.UserName) == EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"Username {userInsertModel.UserName} is already taken");
                return new Output<UserModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Username {userInsertModel.UserName} is already taken. Please change your username and try again"
                };
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = _mapper.Map<User>(userInsertModel);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await AddAsync(user);
            if (await this.SaveChangesAsync())
            {
                return new Output<UserModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<UserModel>(user)
                };
            }
            _logger.LogError($"Error while saving new {typeof(User).Name}. {typeof(User).Name} insert parameters: {JsonSerializer.ToJsonString(userInsertModel)} and Password: {password}");
            return new Output<UserModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Save could not be completed. Please try again later"
            };
        }

        public async Task<Output<AuthenticatedModel>> AuthenticateUserAsync(AuthenticateModel authenticateModel)
        {
            if (authenticateModel.Username.IsNullOrWhiteSpace() || authenticateModel.Password.IsNullOrWhiteSpace())
            {
                _logger.LogWarning($"Username {authenticateModel.Username} or Password are not valid");
                return new Output<AuthenticatedModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Username {authenticateModel.Username} or Password are not valid"
                };
            }

            var user = await FindUserByUsernameAsync(authenticateModel.Username);
            if (user == null)
            {
                _logger.LogWarning($"Username {authenticateModel.Username} is not valid");
                return new Output<AuthenticatedModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Username {authenticateModel.Username} is not valid"
                };
            }

            if (!VerifyPasswordHash(authenticateModel.Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning($"Password {authenticateModel.Username} is not valid");
                return new Output<AuthenticatedModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Password is not valid"
                };
            }
            Role role = await FindRoleByIdUser(user.IdUser);
            if (role == null)
            {
                _logger.LogWarning($"User does not have any available role");
                return new Output<AuthenticatedModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"User does not have any available role"
                };
            }

            AuthenticatedModel userModel;
            try
            {
                userModel = _mapper.Map<AuthenticatedModel>(user);
                userModel.Token = CreateToken(user, role.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}. StackTrace: {ex.StackTrace}");
                throw new EStoreAPIException($"{CommonStatusCodeMessage.InternalServerErrorMessage}");
            }
            return new Output<AuthenticatedModel>()
            {
                Status = EnResultStatus.Success,
                Message = $"Ok",
                Result = userModel
            };
        }

        public async Task<Output<dynamic>> DeleteUserAsync(UserDeleteModel userDeleteModel)
        {
            _logger.LogInformation($"Deleting {typeof(User).Name} with IdUser: {userDeleteModel.IdUser}");
            User user = await FindUserAsync(userDeleteModel.IdUser);
            if (user == null)
            {
                _logger.LogWarning($"Delete {typeof(User).Name} failed. {typeof(User).Name} with IdUser: {userDeleteModel.IdUser} not found");
                return new Output<dynamic>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Delete {typeof(User).Name} failed. {typeof(User).Name} with IdUser: {userDeleteModel.IdUser} not found",
                };
            }
            _mapper.Map(userDeleteModel, user);
            _dbContext.Entry(user).State = EntityState.Modified;
            if (await this.SaveChangesAsync())
            {
                return new Output<dynamic>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok"
                };
            }
            _logger.LogError($"Delete {typeof(User).Name} failed. {typeof(User).Name} with IdUser: {userDeleteModel.IdUser}");
            return new Output<dynamic>()
            {
                Status = EnResultStatus.Error,
                Message = $"Delete could not be completed. Please try again later"
            };
        }

        #endregion

        #region Private Methods

        private async Task<User> FindUserAsync(Guid idUser, bool isActive = true)
        {
            return await _dbContext.User.AsNoTracking().FirstOrDefaultAsync(u => u.IdUser == idUser && u.IsActive == isActive);
        }

        private async Task<User> FindUserByUsernameAsync(string username, bool isActive = true)
        {
            return await _dbContext.User.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username && u.IsActive == isActive);
        }

        private async Task<Role> FindRoleByIdUser(Guid idUser, bool isActive = true)
        {
            var userRole = await _dbContext.UserRole.AsNoTracking().Include(r => r.IdRoleNavigation).FirstOrDefaultAsync(ur => ur.IdUser == idUser && ur.IdRoleNavigation.IsActive == isActive);
            return userRole?.IdRoleNavigation;
        }

        private string CreateToken(User user, string role)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfiguration.Secret));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddSeconds(_jwtConfiguration.ExpireTimeInSeconds),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private bool CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password.IsNullOrWhiteSpace())
            {
                _logger.LogError($"Password {password} cannot be empty or whitespace only string.");
                passwordHash = null;
                passwordSalt = null;
                return false;
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            return true;
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password.IsNullOrWhiteSpace())
            {
                _logger.LogError($"Password {password} cannot be empty or whitespace only string.");
                return false;
            }
            if (storedHash.Length != 64)
            {
                _logger.LogError($"PasswordHash Invalid length of password hash(64 bytes expected).");
                return false;
            }
            if (storedSalt.Length != 128)
            {
                _logger.LogError($"PasswordHash Invalid length of password salt (128 bytes expected).");
                return false;
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }
            return true;
        }

        #endregion

    }
}
