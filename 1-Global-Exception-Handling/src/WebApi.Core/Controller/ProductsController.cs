using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.Model;
using WebApi.Service;

namespace WebApi.Core.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductsService _productsService;
        public ProductController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductCreateModel productModelDTO)
        {
            if (!ModelState.IsValid || productModelDTO is null)
            {
                return BadRequest("Invalid.");
            }

            await _productsService.CreateProductAsync(productModelDTO);
            return Ok();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProductRetrieveModel>> GetProductDetailsByIdAsync(int id)
        {
            var productModelDTO= await _productsService.GetProductDetailsByIdAsync(id);
            if (productModelDTO is null)
            {
                return NotFound("Invalid.");
            }
            else
            {
                return Ok(productModelDTO);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ProductRetrieveModel>> GetProductListAsync()
        {
            return Ok(await _productsService.GetProductListAsync());
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProductDetailsById([FromBody] ProductUpdateModel productModelDTO)
        {
            if (!ModelState.IsValid || productModelDTO is null)
            {
                return BadRequest("Invalid.");
            }

            var productModelDB = await _productsService.GetProductByIdAsync(productModelDTO.ProductID);
            if (productModelDB is null)
            {
                return NotFound("Invalid.");
            }
            else
            {
                await _productsService.UpdateProductDetailsByIdAsync(productModelDTO);
                return NoContent();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteProductDetailsByIdAsync(int id)
        {
            var productModelDB = await _productsService.GetProductByIdAsync(id);
            if (productModelDB is null)
            {
                return NotFound("Invalid.");
            }
            else
            {
                await _productsService.DeleteProductByIdAsync(productModelDB);
                return NoContent();
            }
        }
    }
}