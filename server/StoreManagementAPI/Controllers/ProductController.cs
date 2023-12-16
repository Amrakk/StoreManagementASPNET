using Microsoft.AspNetCore.Mvc;
using StoreManagementAPI.Services;
using StoreManagementAPI.Models;
using StoreManagementAPI.Models.RequestSchemas;
using System.Text.RegularExpressions;
using System.Net;

namespace StoreManagementAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(new ApiResponse<Product>(StatusCodes.Status200OK, "Get all products success", products));
        }

    }
}
