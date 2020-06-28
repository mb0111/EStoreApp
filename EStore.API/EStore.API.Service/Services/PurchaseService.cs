using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Purchase;
using EStore.API.Service.Models.Transaction;
using EStore.API.Service.Models.UserProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Utf8Json;

namespace EStore.API.Service.Services
{
    public class PurchaseService : IPurchaseService
    {
        #region Private Property

        private readonly string ServiceName = nameof(PurchaseService);
        private readonly EStoreContext _dbContext;
        private readonly IUserProductService _userProductService;
        private readonly IMapper _mapper;
        private readonly ILogger<PurchaseService> _logger;

        #endregion

        #region Constructor
        public PurchaseService(EStoreContext dbContext,
                               IUserProductService userProductService,
                               IMapper mapper,
                               ILogger<PurchaseService> logger)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._userProductService = userProductService ?? throw new ArgumentNullException(nameof(userProductService));
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

        public async Task<EnEntityExistsStatus> PurchaseExistsAsync(Guid idPurchase)
        {
            _logger.LogInformation($"Checking if {nameof(Purchase)} with IdPurchase: {idPurchase} exists");
            if (idPurchase.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while checking if {nameof(Purchase)} exists. {nameof(Purchase.IdPurchase)} cannot be empty Guid");
                return EnEntityExistsStatus.BadRequest;
            }
            Purchase purchase = await FindPurchaseAsync(idPurchase);
            return purchase == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        public async Task<Output<EnPurchaseStatus>> GetPurchaseStatusAsync(Guid idPurchase)
        {
            _logger.LogInformation($"Searching for {typeof(Purchase).Name} Status with IdPurchase: {idPurchase}");
            if (idPurchase.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while searching for {typeof(Purchase).Name} with IdPurchase {Guid.Empty}");
                return new Output<EnPurchaseStatus>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdPurchase {idPurchase} is not valid"
                };
            }
            Purchase purchase = await FindPurchaseAsync(idPurchase);
            if (purchase != null)
            {
                return new Output<EnPurchaseStatus>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = (EnPurchaseStatus)purchase.IdPurchaseStatus
                };
            }
            _logger.LogInformation($"No {typeof(Purchase).Name} Status found with IdPurchase: {idPurchase}");
            return new Output<EnPurchaseStatus>()
            {
                Status = EnResultStatus.BadRequest,
                Message = $"Get could not be completed. Please try again later"
            };
        }

        public async Task<Output<PurchaseModel>> GetPurchaseAsync(Guid idPurchase)
        {
            _logger.LogInformation($"Searching for {typeof(Purchase).Name} with IdPurchase: {idPurchase}");
            if (idPurchase.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while searching for {typeof(Purchase).Name} with IdPurchase {Guid.Empty}");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdPurchase {idPurchase} is not valid"
                };
            }
            Purchase purchase = await FindPurchaseAsync(idPurchase);
            if (purchase != null)
            {
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<PurchaseModel>(purchase)
                };
            }
            _logger.LogInformation($"No {typeof(Purchase).Name} found with IdPurchase: {idPurchase}");
            return new Output<PurchaseModel>()
            {
                Status = EnResultStatus.BadRequest,
                Message = $"Get could not be completed. Please try again later"
            };
        }

        public async Task<Output<PurchaseModel>> CreatePurchaseAsync(PurchaseInsertModel purchaseInsertModel)
        {
            if (await _userProductService.UserProductExistsAsync(purchaseInsertModel.IdUserProduct) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdUserProduct {purchaseInsertModel.IdUserProduct} is not valid");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdSellerProduct {purchaseInsertModel.IdUserProduct} is not valid"
                };
            }

            var output = await _userProductService.GetUserProductAsync(purchaseInsertModel.IdUserProduct);
            if (output.Status != EnResultStatus.Success)
            {
                _logger.LogWarning($"IdUserProduct {purchaseInsertModel.IdUserProduct} is not valid");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdSellerProduct {purchaseInsertModel.IdUserProduct} is not valid"
                };
            }
            UserProductModel userProductModel = output.Result;
            if (userProductModel.Quantity < 1)
            {
                _logger.LogWarning($"Purchase for product with IdUserProduct {purchaseInsertModel.IdUserProduct} could not be completed cause there is no available product");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Purchase for product with IdSellerProduct {purchaseInsertModel.IdUserProduct} could not be completed cause there is no available product"
                };
            }
            if (purchaseInsertModel.Price < userProductModel.Price)
            {
                _logger.LogWarning($"Requested purchase is not accepted cause price {purchaseInsertModel.Price} is less than user product min. price {userProductModel.Price}");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"Requested purchase is not accepted cause price {purchaseInsertModel.Price} is less than product min. price {userProductModel.Price}"
                };
            }
            Purchase purchase = _mapper.Map<Purchase>(purchaseInsertModel);
            await AddAsync(purchase);
            if (await this.SaveChangesAsync())
            {
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<PurchaseModel>(purchase)
                };
            }
            _logger.LogError($"Error while saving new {typeof(Purchase).Name}. {typeof(Purchase).Name} insert parameters: {JsonSerializer.ToJsonString(purchaseInsertModel)}");
            return new Output<PurchaseModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Save could not be completed. Please try again later"
            };
        }

        public async Task<Output<PurchaseModel>> UpdatePurchaseAsync(PurchaseUpdateModel purchaseUpdateModel)
        {
            if (await PurchaseExistsAsync(purchaseUpdateModel.IdPurchase) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdPurchase {purchaseUpdateModel.IdPurchase} is not valid");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdPurchase {purchaseUpdateModel.IdPurchase} is not valid"
                };
            }
            if (!Enum.IsDefined(typeof(EnPurchaseStatus), purchaseUpdateModel.IdPurchaseStatus))
            {
                _logger.LogWarning($"IdPurchaseStatus {purchaseUpdateModel.IdPurchaseStatus} is not valid");
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"PurchaseStatus {purchaseUpdateModel.IdPurchaseStatus} is not valid. Allowed values are [{Extensions.GetEnumIntValuesAsStringWithComma<EnPurchaseStatus>()}]."
                };
            }            
            Purchase purchase = await FindPurchaseAsync(purchaseUpdateModel.IdPurchase);
            _mapper.Map(purchaseUpdateModel, purchase);
            _dbContext.Entry(purchase).State = EntityState.Modified;
            if (await this.SaveChangesAsync())
            {
                return new Output<PurchaseModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<PurchaseModel>(purchase)
                };
            }
            _logger.LogError($"Update {typeof(Purchase).Name} error. {typeof(Purchase).Name} with IdPurchase: {purchaseUpdateModel.IdPurchase} and parameters: {JsonSerializer.ToJsonString(purchaseUpdateModel)}");
            return new Output<PurchaseModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Update could not be completed. Please try again later"
            };
        }

        #endregion

        #region Private

        private async Task<Purchase> FindPurchaseAsync(Guid idPurchase)
        {
            return await _dbContext.Purchase.AsNoTracking().FirstOrDefaultAsync(p => p.IdPurchase == idPurchase);
        }

        #endregion
    }
}
