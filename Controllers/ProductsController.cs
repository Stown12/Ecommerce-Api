using AutoMapper;
using Ecommerce_Api.Models;
using Ecommerce_Api.Models.Dtos;
using Ecommerce_Api.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    
    private readonly IMapper _mapper;
 
    public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
    { 
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProducts()
    {
        var products = _productRepository.GetProducts();
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);

        
    }

    [HttpGet("{productId:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductById(int productId)
    {
        var product = _productRepository.GetProduct(productId);
        if (product == null) return NotFound($"Product with ID: {productId} not found");
        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (createProductDto == null) return BadRequest(ModelState); 
        
        if (_productRepository.ProductExists(createProductDto.Name))
        {
            ModelState.AddModelError("CustomError", "Product already exists");
            return BadRequest(ModelState);
        }

        if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", $"Category {createProductDto.CategoryId} not exists");
            return BadRequest(ModelState);
        }
        
        var product = _mapper.Map<Product>(createProductDto);
        if (!_productRepository.CreateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {product.Name}");
            return StatusCode(500, ModelState);
        }
        
        var createProduct = _productRepository.GetProduct(product.ProductId);
        var productDto = _mapper.Map<ProductDto>(createProduct);
        return CreatedAtRoute("GetProduct", new { productId = product.ProductId }, productDto);
    }
    
    [HttpGet("searchProductByCategory/{categoryId:int}", Name = "GetProductForCategory")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsForCategory(int categoryId)
    {
        var products = _productRepository.GetProductsForCategory(categoryId);
        if(!_categoryRepository.CategoryExists(categoryId))
        {
            ModelState.AddModelError("CustomError", $"Category {categoryId} not exists");
            return BadRequest(ModelState);
        }
        if (products.Count == 0 ) return NotFound($"Category with ID: {categoryId} not contains any products");
        
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }
    
    [HttpGet("searchProductByNameDescription/{searchTerm}", Name = "SearchProducts")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SearchProducst(string searchTerm)
    {
        var products = _productRepository.SearchProducts(searchTerm);
        if (products.Count == 0 ) return NotFound($"Products with name or description: {searchTerm} not found");
        
        var productsDto = _mapper.Map<List<ProductDto>>(products);
        return Ok(productsDto);
    }
    
    [HttpPatch("buyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult SearchProducst(string name, int quantity)
    {
        if(string.IsNullOrWhiteSpace(name) || quantity <= 0) return BadRequest("Product name and quantity not be valid");

        var foundProduct = _productRepository.ProductExists(name);
        if(!foundProduct) return NotFound($"Product with name: {name} not found");
        
        if (!_productRepository.BuyProduct(name, quantity))
        {
            ModelState.AddModelError("CustomError", $"Product {name} not buying");
            return BadRequest(ModelState);
        }
        
        return Ok($"Product {name} bought successfully. Quantity: {quantity}");
    }
    
    [HttpPut("{productId:int}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
    {
        if (updateProductDto == null) return BadRequest(ModelState); 

        if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
        {
            ModelState.AddModelError("CustomError", $"Category {updateProductDto.CategoryId} not exists");
            return BadRequest(ModelState);
        }
        
        var product = _mapper.Map<Product>(updateProductDto);
        product.ProductId = productId;
        if (!_productRepository.UpdateProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong when updating the record {product.Name}");
            return StatusCode(500, ModelState);
        }
        
        return NoContent();
    }
    
    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        if(productId == 0) return BadRequest(ModelState);

        var product = _productRepository.GetProduct(productId);
        if (product == null)
        {
            return NotFound($"Product with ID: {productId} not exists");
        }
        if (!_productRepository.DeleteProduct(product))
        {
            ModelState.AddModelError("CustomError", $"Something went wrong when deleting the record {product.Name}");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
}