using AutoMapper;
using EStore.API.Controllers.Base;
using EStore.API.Core.Authorization;
using EStore.API.DTO.Header;
using EStore.API.DTO.Search;
using EStore.API.DTO.UserProduct;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Common;
using EStore.API.Service.Models.Configuration;
using EStore.API.Service.Models.Product;
using EStore.API.Service.Models.Search;
using EStore.API.Service.Models.UserProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utf8Json;

namespace EStore.API.Controllers
{
    /// <summary>
    /// UserProduct
    /// </summary>
    [RoleAuthorize(EnRole.Seller, EnRole.Byer)]
    [Route("api/ProductInfo")]
    [ApiController]
    public class UserProductController : EStoreAPIControllerBase
    {
        #region Private Property
        private readonly IUserProductService _userProductService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly PaginationConfiguration _paginationConfiguration;
        #endregion

        #region Constructor
        /// <summary>
        /// UserProduct Controller
        /// </summary>
        /// <param name="userProductService"></param>
        /// <param name="productService"></param>
        /// <param name="categoryService"></param>
        /// <param name="mapper"></param>
        /// <param name="options"></param>
        public UserProductController(IUserProductService userProductService,
                                     IProductService productService,
                                     ICategoryService categoryService,
                                     IMapper mapper,
                                     IOptions<PaginationConfiguration> options)
        {
            this._userProductService = userProductService ?? throw new ArgumentNullException(nameof(userProductService));
            this._productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this._categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._paginationConfiguration = options.Value;
        }
        #endregion

        /// <summary>
        /// Get Paged ProductInfo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductInfoDTO>>> Get([FromQuery] UserProductSearchCriteriaDTO dtoModel)
        {
            if (Role.IsSeller())
            {
                if (!CanSellerContinueSearch(dtoModel.IdSeller))
                    return BadRequest($"IdSeller {dtoModel.IdSeller} is not valid");
                if (!dtoModel.IdSeller.HasValue)
                    dtoModel.IdSeller = IdUser;
            }
            _mapper.Map(PaginationBaseDTO.EvaluatePaginationWithConfig(dtoModel, _paginationConfiguration), dtoModel);
            UserProductSearchCriteria searchCriteria = _mapper.Map<UserProductSearchCriteria>(dtoModel);
            searchCriteria.Role = Role;
            Output<PagedList<ProductInfoModel>> output = await _userProductService.SearchUserProductAsync(searchCriteria);
            if (output.Status == EnResultStatus.Success)
            {
                PagedList<ProductInfoModel> pagedList = output.Result;
                if (!pagedList.IsNullOrEmpty())
                {
                    Response.Headers.Add("X-Pagination",
                        JsonSerializer.ToJsonString(_mapper.Map<PaginationMetadataDTO>(pagedList)));

                    return Ok(_mapper.Map<IEnumerable<ProductInfoDTO>>(pagedList.ToList()));
                }
                else
                    return NoContent();
            }
            else if (output.Status == EnResultStatus.Error)
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            return BadRequest($"{output.Message}");
        }

        /// <summary>
        /// Create new ProductInfo
        /// </summary>
        /// <param name="dtoModel">ProductInfo insert data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserProductDTO>> Post([FromForm] UserProductInsertDTO dtoModel)
        {
            try
            {
                ProductInsertModel productInsertModel = _mapper.Map<ProductInsertModel>(dtoModel);
                productInsertModel.InsertBy = IdUser;
                var output = await _productService.CreateProductAsync(productInsertModel);
                if (output.Status == EnResultStatus.Success)
                {
                    UserProductInsertModel userProductInsertModel = _mapper.Map<UserProductInsertModel>(dtoModel);
                    userProductInsertModel.IdProduct = output.Result.IdProduct;
                    userProductInsertModel.IdUser = IdUser;
                    userProductInsertModel.InsertBy = IdUser;
                    var outputUserProduct = await _userProductService.CreateUserProductAsync(userProductInsertModel);
                    if (outputUserProduct.Status == EnResultStatus.Success)
                    {
                        return CreatedAtRoute("GetProduct", new { controller = "Product", idProduct = outputUserProduct.Result.IdProduct }, _mapper.Map<UserProductDTO>(userProductInsertModel));
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
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

        #region Private

        private bool CanSellerContinueSearch(Guid? idUser)
        {
            return !idUser.HasValue || IdUser == idUser.Value;
        }

        #endregion
    }
}
