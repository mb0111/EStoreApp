using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EStore.API.Service.Services
{
    public class CategoryService : ICategoryService
    {
        #region Private Property

        private readonly EStoreContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        #endregion

        #region Constructor

        public CategoryService(EStoreContext dbContext,
                               IMapper mapper,
                               ILogger<CategoryService> logger)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Service Implementation

        public async Task<EnEntityExistsStatus> CategoryExistsAsync(Guid idCategory)
        {
            _logger.LogInformation($"Checking if {typeof(Category).Name} with IdCategory: {idCategory} exists");
            if (idCategory.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while checking if {typeof(Category).Name} exists. {typeof(Category).Name} with IdCategory Empty Guid is not valid");
                return EnEntityExistsStatus.BadRequest;
            }
            Category category = await FindCategoryAsync(idCategory);
            return category == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        #endregion

        #region Private Methods

        private async Task<Category> FindCategoryAsync(Guid idCategory, bool isActive = true)
        {
            return await _dbContext.Category.AsNoTracking().FirstOrDefaultAsync(c => c.IdCategory == idCategory && c.IsActive == isActive);
        }

        #endregion
    }
}
