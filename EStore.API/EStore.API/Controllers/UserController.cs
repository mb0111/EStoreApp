using AutoMapper;
using EStore.API.Controllers.Base;
using EStore.API.Core.Authorization;
using EStore.API.DTO;
using EStore.API.DTO.Authenticate;
using EStore.API.DTO.User;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Authenticate;
using EStore.API.Service.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EStore.API.Controllers
{
    /// <summary>
    /// User
    /// </summary>
    [RoleAuthorize(EnRole.Byer, EnRole.Seller)]
    [ApiController]
    [Route("api/User")]
    public class UserController : EStoreAPIControllerBase
    {
        #region Private Property
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// User Controller
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        public UserController(IUserService userService,
                                 IMapper mapper)
        {
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        /// <summary>
        /// Create new User
        /// </summary>
        /// <param name="dtoModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] UserInsertDTO dtoModel)
        {
            try
            {
                UserInsertModel userInsert = _mapper.Map<UserInsertModel>(dtoModel);
                userInsert.InsertBy = IdUser;
                var result = await _userService.CreateUserAsync(userInsert, dtoModel.Password);
                if (result.Status == EnResultStatus.Success)
                    return Ok(_mapper.Map<UserDTO>(result.Result));
                else if (result.Status == EnResultStatus.Error)
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                return BadRequest($"{result.Message}");
            }
            catch (EStoreAPIException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            }
        }

        /// <summary>
        /// Authenticate User
        /// </summary>
        /// <param name="dtoModel">Authenticate Username and Password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedDTO>> LogIn([FromBody] AuthenticateDTO dtoModel)
        {
            try
            {
                var result = await _userService.AuthenticateUserAsync(_mapper.Map<AuthenticateModel>(dtoModel));
                if (result.Status == EnResultStatus.Success)
                    return Ok(_mapper.Map<AuthenticatedDTO>(result.Result));
                else if (result.Status == EnResultStatus.Error)
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                return BadRequest($"{result.Message}");
            }
            catch (EStoreAPIException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            }
        }
    }
}
