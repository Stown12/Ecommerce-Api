using Ecommerce_Api.Models;

namespace Ecommerce_Api.Repository.IRepository;

public interface IProductRepository
{
    public ICollection<Product> GetProducts();
    public ICollection<Product> GetProductsForCategory(int categoryId);
    public ICollection<Product> SearchProducts(string name);
    public Product? GetProduct(int id);
    public bool BuyProduct(string name, int quantity);
    public bool ProductExists(int id);
    public bool ProductExists(string name);
    public bool CreateProduct(Product product);
    public bool UpdateProduct(Product product);
    public bool DeleteProduct(Product product);
    public bool Save();
}