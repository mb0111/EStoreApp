using AutoMapper;
using EStore.API.Controllers.Base;
using EStore.API.Core.Authorization;
using EStore.API.DTO.Transaction;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.API.Controllers
{
    /// <summary>
    /// Transaction
    /// </summary>
    [RoleAuthorize()]
    [ApiController]
    [Route("api/Transaction")]
    public class TransactionController : EStoreAPIControllerBase
    {
        #region Private Property
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Transaction Controller
        /// </summary>
        /// <param name="transactionService"></param>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        public TransactionController(ITransactionService transactionService,
                                     IUserService userService,
                                     IMapper mapper)
        {
            this._transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="dtoModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> Get([FromQuery] TransactionSearchSellerByerDTO dtoModel)
        {
            if (await _userService.UserExistsAsync(dtoModel.IdSeller) != EnEntityExistsStatus.Found)
            {
                return BadRequest($"IdSeller {dtoModel.IdSeller} is not valid");
            }
            if (await _userService.UserExistsAsync(dtoModel.IdByer) != EnEntityExistsStatus.Found)
            {
                return BadRequest($"IdByer {dtoModel.IdByer} is not valid");
            }
            var output = await _transactionService.GetTransactionsForSellerAndByerAsync(_mapper.Map<TransactionSearchSellerByerModel>(dtoModel));
            if (output.Status == EnResultStatus.Success)
                return Ok(_mapper.Map<IEnumerable<TransactionDTO>>(output.Result));
            else if (output.Status == EnResultStatus.Error)
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            else if (output.Status == EnResultStatus.NotFound)
                return NotFound($"{output.Message}");
            return BadRequest($"{output.Message}");
        }

    }
}
