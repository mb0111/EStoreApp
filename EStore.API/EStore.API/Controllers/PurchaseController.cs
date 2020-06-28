using AutoMapper;
using EStore.API.Controllers.Base;
using EStore.API.Core.Authorization;
using EStore.API.DTO.Purchase;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Purchase;
using EStore.API.Service.Models.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EStore.API.Controllers
{
    /// <summary>
    /// Product
    /// </summary>
    [RoleAuthorize(EnRole.Seller, EnRole.Byer)]
    [Route("api/Purchase")]
    [ApiController]
    public class PurchaseController : EStoreAPIControllerBase
    {
        #region Private Property
        private readonly IPurchaseService _purchaseService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Purchase Controller
        /// </summary>
        /// <param name="purchaseService"></param>
        /// <param name="mapper"></param>
        public PurchaseController(IPurchaseService purchaseService,
                                 IMapper mapper)
        {
            this._purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        /// <summary>
        /// Get Purchase
        /// </summary>
        /// <returns></returns>
        [HttpGet("{idPurchase}", Name = "GetPurchase")]
        public async Task<ActionResult<PurchaseDTO>> Get(Guid idPurchase)
        {
            var output = await _purchaseService.GetPurchaseAsync(idPurchase);
            if (output.Status == EnResultStatus.Success)
                return Ok(_mapper.Map<PurchaseDTO>(output.Result));
            else if (output.Status == EnResultStatus.Error)
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            return BadRequest($"{output.Message}");
        }

        /// <summary>
        /// Purchase a Product
        /// </summary>
        /// <param name="dtoModel"> Purchase data</param>
        /// <returns></returns>
        [HttpPost]
        [RoleAuthorize(EnRole.Byer)]
        public async Task<ActionResult<PurchaseDTO>> Post([FromBody] PurchaseInsertDTO dtoModel)
        {
            try
            {
                PurchaseInsertModel purchaseInsertModel = _mapper.Map<PurchaseInsertModel>(dtoModel);
                purchaseInsertModel.InsertBy = IdUser;
                var output = await _purchaseService.CreatePurchaseAsync(purchaseInsertModel);
                if (output.Status == EnResultStatus.Success)
                {
                    return CreatedAtRoute("GetPurchase", new { idPurchase = output.Result.IdPurchase }, _mapper.Map<PurchaseDTO>(output.Result));
                }
                else if (output.Status == EnResultStatus.Error)
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                return BadRequest($"{output.Message}");
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
        /// Update Purchase
        /// </summary>
        /// <param name="idPurchase">IdPurchase</param>
        /// <param name="dtoModel">PurchaseUpdate data</param>
        /// <param name="_transactionService"></param>
        [RoleAuthorize(EnRole.Seller)]
        [HttpPut("{idPurchase}")]
        public async Task<ActionResult<PurchaseDTO>> Put(Guid idPurchase, [FromBody] PurchaseUpdateDTO dtoModel, [FromServices] ITransactionService _transactionService)
        {
            if (!idPurchase.Equals(dtoModel.IdPurchase))
            {
                return BadRequest($"IdPurchase values must be equal");
            }
            if (!Enum.IsDefined(typeof(EnPurchaseStatus), dtoModel.PurchaseStatus))
            {
                return BadRequest($"PurchaseStatus {dtoModel.PurchaseStatus} is not valid.");
            }
            EnPurchaseStatus updatePurchaseStatus = (EnPurchaseStatus)dtoModel.PurchaseStatus;
            if (Role.IsSeller() && !updatePurchaseStatus.IsValidPurchaseStatusForSeller())
            {
                return BadRequest($"PurchaseStatus {dtoModel.PurchaseStatus} is not valid.");
            }
            try
            {
                var purchaseStatusOutput = await _purchaseService.GetPurchaseStatusAsync(idPurchase);
                if (purchaseStatusOutput.Status != EnResultStatus.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                }
                EnPurchaseStatus currentPurchaseStatus = purchaseStatusOutput.Result;
                PurchaseUpdateModel purchaseUpdateModel = _mapper.Map<PurchaseUpdateModel>(dtoModel);
                purchaseUpdateModel.UpdateBy = IdUser;
                var purchaseOutput = await _purchaseService.UpdatePurchaseAsync(purchaseUpdateModel);
                if (purchaseOutput.Status == EnResultStatus.Success)
                {
                    // If Confirm the Purchase that was in StandBy, then Create new Transaction
                    if (currentPurchaseStatus.Equals(EnPurchaseStatus.StandBy) && updatePurchaseStatus.Equals(EnPurchaseStatus.Confirmed))
                    {
                        TransactionInsertModel transactionInsertModel = _mapper.Map<TransactionInsertModel>(purchaseUpdateModel);
                        transactionInsertModel.Amount = (purchaseOutput.Result.Quantity * purchaseOutput.Result.Price);
                        var transactionOutput = await _transactionService.CreateTransactionAsync(transactionInsertModel);
                        if (purchaseOutput.Status != EnResultStatus.Success)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                        }
                    }
                    return Ok(_mapper.Map<PurchaseDTO>(purchaseOutput.Result));
                }
                else if (purchaseOutput.Status == EnResultStatus.Error)
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                return BadRequest($"{purchaseOutput.Message}");
            }
            catch (EStoreAPIException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            }
        }

        /// <summary>
        /// Cancel Purchase
        /// </summary>
        /// <param name="idPurchase">IdPurchase</param>
        [RoleAuthorize(EnRole.Byer)]
        [HttpPut("{idPurchase}/Cancel")]
        public async Task<ActionResult<PurchaseDTO>> CancelPurchase(Guid idPurchase)
        {
            try
            {
                var purchaseStatusOutput = await _purchaseService.GetPurchaseStatusAsync(idPurchase);
                if (purchaseStatusOutput.Status != EnResultStatus.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                }
                EnPurchaseStatus currentPurchaseStatus = purchaseStatusOutput.Result;
                if (currentPurchaseStatus.Equals(EnPurchaseStatus.StandBy))
                {
                    PurchaseUpdateDTO dtoModel = new PurchaseUpdateDTO()
                    {
                        IdPurchase = idPurchase,
                        PurchaseStatus = (int)EnPurchaseStatus.Cancelled
                    };
                    PurchaseUpdateModel purchaseUpdateModel = _mapper.Map<PurchaseUpdateModel>(dtoModel);
                    purchaseUpdateModel.UpdateBy = IdUser;
                    var purchaseOutput = await _purchaseService.UpdatePurchaseAsync(purchaseUpdateModel);
                    if (purchaseOutput.Status == EnResultStatus.Success)
                    {
                        return Ok(_mapper.Map<PurchaseDTO>(purchaseOutput.Result));
                    }
                    else if (purchaseOutput.Status == EnResultStatus.Error)
                        return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
                    return BadRequest($"{purchaseOutput.Message}");
                }
                else
                    return BadRequest($"Purchase is {Enum.GetName(typeof(EnPurchaseStatus), currentPurchaseStatus)} and cannot be cancelled");
            }
            catch (EStoreAPIException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            }
        }

    }
}
