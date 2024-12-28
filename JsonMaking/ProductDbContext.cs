using Microsoft.EntityFrameworkCore;

namespace JsonMaking
{
	public class ProductDbContext : DbContext
	{
		public DbSet<Product> Products { get; set; }
		public ProductDbContext() { }
		protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("DataSource = products.db");
	}
}