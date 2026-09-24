using Ecommerce_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Api.Data;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Category> Categories { get; set; }
}