using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     // api is optional, but it's conventional to do it
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
  
        public ProductsController(IGenericRepository<Product> productsRepo, 
                    IGenericRepository<ProductBrand> productBrandRepo,
                    IGenericRepository<ProductType> productTypeRepo) 
        {
            _productsRepo = productsRepo;
            _productTypeRepo = productTypeRepo;
            _productBrandRepo = productBrandRepo;
            
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();
            //var products = await _productsRepo.ListAllAsync();
            var products = await _productsRepo.ListAsync(spec);
            return Ok(products);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            return (await _productsRepo.GetEntityWithSpec(spec));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productsRepo.ListAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            var types = await _productsRepo.ListAllAsync();
            return Ok(types);
        }
    }
}