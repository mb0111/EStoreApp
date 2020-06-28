using AutoMapper;
using EStore.API.Controllers.Base;
using EStore.API.Core.Authorization;
using EStore.API.DTO.Product;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Product;
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
    [ApiController]
    [Route("api/Product")]
    public class ProductController : EStoreAPIControllerBase
    {
        #region Private Property
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        /// <summary>
        /// Product Controller
        /// </summary>
        /// <param name="productService"></param>
        /// <param name="categoryService"></param>
        /// <param name="mapper"></param>
        public ProductController(IProductService productService,
                                 ICategoryService categoryService,
                                 IMapper mapper)
        {
            this._productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this._categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="idProduct"></param>
        /// <returns></returns>
        [HttpGet("{idProduct}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDTO>> Get(Guid idProduct)
        {
            var output = await _productService.GetProductAsync(idProduct);
            if (output.Status == EnResultStatus.Success)
                return Ok(_mapper.Map<ProductDTO>(output.Result));
            else if (output.Status == EnResultStatus.Error)
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            return BadRequest($"{output.Message}");
        }

        /// <summary>
        /// Download Product Image
        /// </summary>
        /// <param name="idProduct"></param>
        /// <param name="mimeMappingService"></param>
        /// <returns></returns>
        [HttpGet("{idProduct}/DownloadImage", Name = "GetProductImage")]
        public async Task<IActionResult> GetImage(Guid idProduct, [FromServices] IMimeMappingService mimeMappingService)
        {
            var output = await _productService.GetProductImageAsync(idProduct);
            if (output.Status == EnResultStatus.Success)
            {
                return File(output.Result.ImageFileData, mimeMappingService.Map(output.Result.ImageFileName), output.Result.ImageFileName);
            }
            else if (output.Status == EnResultStatus.Error)
                return StatusCode(StatusCodes.Status500InternalServerError, CommonStatusCodeMessage.InternalServerErrorMessage);
            return NotFound();
        }

        /// <summary>
        /// Create new Product
        /// </summary>
        /// <param name="dtoModel"> Product insert data</param>
        /// <returns></returns>
        [HttpPost]
        [RoleAuthorize(EnRole.Seller)]
        public async Task<ActionResult<ProductDTO>> Post([FromForm] ProductInsertDTO dtoModel)
        {
            try
            {
                ProductInsertModel productInsertModel = _mapper.Map<ProductInsertModel>(dtoModel);
                productInsertModel.InsertBy = IdUser;
                var output = await _productService.CreateProductAsync(productInsertModel);
                if (output.Status == EnResultStatus.Success)
                {
                    return CreatedAtRoute("GetProduct", new { idProduct = output.Result.IdProduct }, _mapper.Map<ProductDTO>(output.Result));
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
        /// Delete Product
        /// </summary>
        /// /// <param name="idProduct"></param>
        /// <param name="_userProductService"></param>
        /// <returns></returns>
        [HttpDelete("{idProduct}")]
        [RoleAuthorize()]
        public async Task<ActionResult<ProductDTO>> Delete(Guid idProduct, [FromServices] IUserProductService _userProductService)
        {
            try
            {
                var output = await _productService.DeleteProductAsync(new ProductDeleteModel()
                {
                    IdProduct = idProduct,
                    IsActive = false,
                    DeleteBy = IdUser,
                    DeleteDate = DateTime.UtcNow
                });
                if (output.Status == EnResultStatus.Success)
                    return Ok();
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
    }
}
