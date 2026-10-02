using Ecommerce_Api.Data;
using Ecommerce_Api.Models;
using Ecommerce_Api.Repository.IRepository;

namespace Ecommerce_Api.Repository;

public class CategoryRepository: ICategoryRepository
{
    private readonly ApplicationDbContext _db;
    
    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public ICollection<Category> GetCategories()
    {
        return _db.Categories.OrderBy(c => c.Name).ToList();
    }

    public Category GetCategory(int id)
    {
        return _db.Categories.FirstOrDefault(c => c.Id == id) ?? throw new InvalidOperationException($"Category with ID: {id} not found");
    }

    public bool CategoryExists(int id)
    {
        return _db.Categories.Any(c => c.Id == id);
    }

    public bool CategoryExists(string name)
    {
        return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        
    }

    public bool CreateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _db.Categories.Add(category);
        return Save();
    }

    public bool UpdateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _db.Categories.Update(category);
        return Save();
    }

    public bool DeleteCategory(Category category)
    {
        _db.Categories.Remove(category);
        return Save();
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }
}