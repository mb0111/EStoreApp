using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Common;
using EStore.API.Service.Models.Search;
using EStore.API.Service.Models.UserProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utf8Json;

namespace EStore.API.Service.Services
{
    public class UserProductService : IUserProductService
    {
        #region Private Property

        private readonly string ServiceName = nameof(ProductService);
        private readonly EStoreContext _dbContext;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProductService> _logger;

        #endregion

        #region Constructor
        public UserProductService(EStoreContext dbContext,
                                  IUserService userService,
                                  IProductService productService,
                                  IMapper mapper,
                                  ILogger<UserProductService> logger)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this._productService = productService ?? throw new ArgumentNullException(nameof(productService));
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

        public async Task<EnEntityExistsStatus> UserProductExistsAsync(Guid idUserProduct)
        {
            _logger.LogInformation($"Checking if {nameof(UserProduct)} with IdUserProduct: {idUserProduct} exists");
            if (idUserProduct.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while checking if {nameof(UserProduct)} exists. {nameof(UserProduct.IdUser)} cannot be empty Guid");
                return EnEntityExistsStatus.BadRequest;
            }
            UserProduct userProduct = await FindUserProductAsync(idUserProduct);
            return userProduct == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        public async Task<Output<UserProductModel>> GetUserProductAsync(Guid idUserProduct)
        {
            UserProduct userProduct = await FindUserProductAsync(idUserProduct);
            if (userProduct != null)
            {
                return new Output<UserProductModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<UserProductModel>(userProduct)
                };
            }
            _logger.LogInformation($"No {typeof(UserProduct).Name} found with IdUserProduct: {idUserProduct}");
            return new Output<UserProductModel>()
            {
                Status = EnResultStatus.BadRequest,
                Message = $"IdUserProduct {idUserProduct} is not valid"
            };
        }

        public async Task<Output<PagedList<ProductInfoModel>>> SearchUserProductAsync(UserProductSearchCriteria searchCriteria)
        {
            if (searchCriteria.IdUser.HasValue && await _userService.UserExistsAsync(searchCriteria.IdUser.Value) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdUser {searchCriteria.IdUser.Value} is not valid");
                return new Output<PagedList<ProductInfoModel>>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdUser {searchCriteria.IdUser.Value} is not valid"
                };
            }

            if (searchCriteria.IdProduct.HasValue && await _productService.ProductExistsAsync(searchCriteria.IdProduct.Value) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdProduct {searchCriteria.IdProduct.Value} is not valid");
                return new Output<PagedList<ProductInfoModel>>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdProduct {searchCriteria.IdProduct.Value} is not valid"
                };
            }

            IQueryable<UserProduct> query = _dbContext.UserProduct.AsNoTracking();
            query = searchCriteria.ApplySearchCriteriaFilter(query);
            PagedList<UserProduct> pagedUserProducts = await PagedList<UserProduct>.CreateAsync(query, searchCriteria.PageNumber, searchCriteria.PageSize);
            List<UserProduct> userProducts = pagedUserProducts.ToList();
            if (!userProducts.IsNullOrEmpty())
            {
                return new Output<PagedList<ProductInfoModel>>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = new PagedList<ProductInfoModel>(_mapper.Map<List<ProductInfoModel>>(userProducts), pagedUserProducts.TotalCount, pagedUserProducts.CurrentPage, pagedUserProducts.PageSize)
                };
            }
            _logger.LogWarning($"No result found. Searching for {typeof(UserProduct).Name} with search parameters: {JsonSerializer.ToJsonString(searchCriteria)}");
            return new Output<PagedList<ProductInfoModel>>()
            {
                Status = EnResultStatus.NotFound,
                Message = $"No result found"
            };
        }

        public async Task<Output<UserProductModel>> CreateUserProductAsync(UserProductInsertModel userProductInsertModel)
        {
            if (await _userService.UserExistsAsync(userProductInsertModel.IdUser) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdUser {userProductInsertModel.IdUser} is not valid");
                return new Output<UserProductModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdUser {userProductInsertModel.IdUser} is not valid"
                };
            }
            if (await _productService.ProductExistsAsync(userProductInsertModel.IdProduct) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdProduct {userProductInsertModel.IdProduct} is not valid");
                return new Output<UserProductModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdProduct {userProductInsertModel.IdProduct} is not valid"
                };
            }
            UserProduct userProduct = _mapper.Map<UserProduct>(userProductInsertModel);
            await AddAsync(userProduct);
            if (await this.SaveChangesAsync())
            {
                return new Output<UserProductModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<UserProductModel>(userProduct)
                };
            }
            _logger.LogError($"Error while saving new {typeof(UserProduct).Name}. {typeof(UserProduct).Name} insert parameters: {JsonSerializer.ToJsonString(userProductInsertModel)}");
            return new Output<UserProductModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Save could not be completed. Please try again later"
            };
        }

        #endregion

        #region Private Method

        private async Task<UserProduct> FindUserProductAsync(Guid idUserProduct, bool isActive = true)
        {
            return await _dbContext.UserProduct.AsNoTracking().FirstOrDefaultAsync(up => up.IdUserProduct == idUserProduct && up.IsActive == isActive);
        }

        #endregion
    }
}
