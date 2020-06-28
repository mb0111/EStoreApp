using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Product;
using EStore.API.Service.Models.UserProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utf8Json;

namespace EStore.API.Service.Services
{
    public class ProductService : IProductService
    {
        #region Private Property

        private readonly string ServiceName = nameof(ProductService);
        private readonly EStoreContext _dbContext;
        private readonly ICategoryService _categoryService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        #endregion

        #region Constructor
        public ProductService(EStoreContext dbContext,
                              ICategoryService categoryService,
                              IFileUploadService fileUploadService,
                              IMapper mapper,
                              ILogger<ProductService> logger)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            this._fileUploadService = fileUploadService ?? throw new ArgumentNullException(nameof(fileUploadService));
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

        public async Task<EnEntityExistsStatus> ProductExistsAsync(Guid idProduct)
        {
            _logger.LogInformation($"Checking if {nameof(Product)} with IdUser: {idProduct} exists");
            if (idProduct.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while checking if {nameof(Product)} exists. {nameof(Product.IdProduct)} cannot be empty Guid");
                return EnEntityExistsStatus.BadRequest;
            }
            Product product = await FindProductAsync(idProduct);
            return product == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        public async Task<Output<ProductModel>> GetProductAsync(Guid idProduct)
        {
            Product product = await FindProductAsync(idProduct);
            if (product != null)
            {
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<ProductModel>(product)
                };
            }
            _logger.LogInformation($"No {typeof(Product).Name} found with IdProduct: {idProduct}");
            return new Output<ProductModel>()
            {
                Status = EnResultStatus.BadRequest,
                Message = $"IdProduct {idProduct} is not valid"
            };
        }

        public async Task<Output<ProductImageModel>> GetProductImageAsync(Guid idProduct)
        {
            Product product = await FindProductAsync(idProduct);
            if (product != null)
            {
                return new Output<ProductImageModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<ProductImageModel>(product)
                };
            }
            return new Output<ProductImageModel>()
            {
                Status = EnResultStatus.BadRequest,
                Message = $"IdProduct {idProduct} is not valid"
            };
        }

        public async Task<Output<ProductModel>> CreateProductAsync(ProductInsertModel productInsertModel)
        {
            if (await _categoryService.CategoryExistsAsync(productInsertModel.IdCategory) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdCategory {productInsertModel.IdCategory} is not valid");
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdCategory {productInsertModel.IdCategory} is not valid"
                };
            }
            var fileOutput = await _fileUploadService.ReadFileAsync(productInsertModel);
            if (fileOutput.Status != EnResultStatus.Success)
            {
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.Error,
                    Message = $"Save could not be completed. Could not upload file. Please try again later"
                };
            }

            Product product = _mapper.Map<Product>(productInsertModel);
            product.ImageFileData = fileOutput.Result.Data;
            product.ImageFileName = fileOutput.Result.Name;
            await AddAsync(product);
            if (await this.SaveChangesAsync())
            {
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<ProductModel>(product)
                };
            }
            _logger.LogError($"Error while saving new {typeof(Product).Name}. {typeof(Product).Name} insert parameters: {JsonSerializer.ToJsonString(productInsertModel)}");
            return new Output<ProductModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Save could not be completed. Please try again later"
            };
        }

        public async Task<Output<ProductModel>> UpdateProductAsync(ProductUpdateModel productUpdateModel)
        {
            if (productUpdateModel.IdProduct.IsEmptyGuid())
            {
                _logger.LogWarning($"IdProduct {productUpdateModel.IdProduct} is not valid");
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdProduct {productUpdateModel.IdCategory} is not valid"
                };
            }

            if (await _categoryService.CategoryExistsAsync(productUpdateModel.IdCategory) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdCategory {productUpdateModel.IdCategory} is not valid");
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdCategory {productUpdateModel.IdCategory} is not valid"
                };
            }
            Product product = await FindProductAsync(productUpdateModel.IdProduct);
            if (product == null)
            {
                _logger.LogWarning($"IdProduct {productUpdateModel.IdProduct} is not valid. No product found with this id.");
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdProduct {productUpdateModel.IdCategory} is not valid"
                };
            }
            _mapper.Map(productUpdateModel, product);
            _dbContext.Entry(product).State = EntityState.Modified;
            if (await this.SaveChangesAsync())
            {
                return new Output<ProductModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<ProductModel>(product)
                };
            }
            _logger.LogError($"Error while udating {typeof(Product).Name}. {typeof(Product).Name} update parameters: {JsonSerializer.ToJsonString(productUpdateModel)}");
            return new Output<ProductModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Update could not be completed. Please try again later"
            };
        }

        public async Task<Output<dynamic>> DeleteProductAsync(ProductDeleteModel productDeleteModel)
        {
            _logger.LogWarning($"Deleting {typeof(Product).Name} with IdProduct: {productDeleteModel.IdProduct}");
            Product product = await _dbContext.Product.Include(up => up.UserProduct).FirstOrDefaultAsync(p => p.IdProduct == productDeleteModel.IdProduct);
            if (product == null)
            {
                _logger.LogWarning($"Delete {typeof(Product).Name} failed. {typeof(Product).Name} with IdProduct: {productDeleteModel.IdProduct} not found");
                return new Output<dynamic>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"{typeof(Product).Name} with IdProduct: {productDeleteModel.IdProduct} not found"
                };
            }
            _mapper.Map(productDeleteModel, product);
            if (!product.UserProduct.IsNullOrEmpty())
            {
                UserProductDeleteModel userProductDeleteModel = _mapper.Map<UserProductDeleteModel>(productDeleteModel);
                product.UserProduct.ToList().ForEach(up =>
                {
                    _mapper.Map(userProductDeleteModel, up);
                });
            }
            _dbContext.Entry(product).State = EntityState.Modified;
            if (await this.SaveChangesAsync())
            {
                return new Output<dynamic>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok"
                };
            }
            _logger.LogError($"Delete {typeof(Product).Name} error. {typeof(Product).Name} with IdProduct: {productDeleteModel.IdProduct}");
            return new Output<dynamic>()
            {
                Status = EnResultStatus.Error,
                Message = $"Delete could not be completed. Please try again later"
            };
        }

        #endregion

        #region Private Methods

        private async Task<Product> FindProductAsync(Guid idProduct, bool isActive = true)
        {
            return await _dbContext.Product.AsNoTracking().FirstOrDefaultAsync(p => p.IdProduct == idProduct && p.IsActive == isActive);
        }

        #endregion

    }
}
