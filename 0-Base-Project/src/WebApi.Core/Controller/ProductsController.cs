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
            try
            {
                await _productsService.CreateProductAsync(productModelDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProductRetrieveModel>> GetProductDetailsByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<ProductRetrieveModel>> GetProductListAsync()
        {
            try
            {
                return Ok(await _productsService.GetProductListAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error. {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProductDetailsById([FromBody] ProductUpdateModel productModelDTO)
        {
            if (!ModelState.IsValid || productModelDTO is null)
            {
                return BadRequest("Invalid.");
            }
            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteProductDetailsByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}