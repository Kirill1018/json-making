
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Drawing;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
namespace JsonMaking
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<ProductDbContext>();
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();
			app.MapGet("/product/all", async (ProductDbContext db) => { return await db.Products.ToArrayAsync(); });
			app.MapGet("/product/{id:int}", async (ProductDbContext db, int id) => { return await db.Products.FindAsync(id); });
			app.MapPost("/product", async (ProductDbContext db, [FromBody] Product product) => {
				db.Products.Add(product);
				await db.SaveChangesAsync();
			});
			app.MapPut("/product", async (ProductDbContext db, [FromBody] Product updProd) =>
			{
				Product? product = await db.Products.FirstOrDefaultAsync(produce => produce.Id == updProd
				.Id);//поиск товара
				if (product == null) return null;
				product.Id = updProd.Id;//изменение идентификатора
				product.Name = updProd.Name;//изменение названия
				product.Price = updProd.Price;//изменение цены
				return product;
			});
			app.MapDelete("/product/{id:int}", async (ProductDbContext db, int id) =>
			{
				DbSet<Product> dbSet = db.Products;//коллекция товаров
				Product? product = await dbSet.FirstOrDefaultAsync(produce => produce.Id == id);//поиск товара
				dbSet.Remove(product!);
				db.Products = dbSet;//обновление коллекции товаров
				await db.SaveChangesAsync();
			});
			app.MapGet("/product/search/{name}", async (ProductDbContext db, string? name) =>
			{
				DbSet<Product> dbSet = db.Products;//коллекция товаров
				string? result = null;//получаемое название для поиска (в начале отсутствует)
				foreach (Product product in dbSet)
				{
					string? behalf = product.Name;//название товара
					if (behalf.Contains(name!)) result = behalf;//присвоение названия для поиска
				}
				if (result == null) return null;
				else return await dbSet.FirstOrDefaultAsync(produce => produce.Name == result);
			});
			app.MapGet("/product/page/{line1:int}/{line2:int}", async (ProductDbContext db, int line1, int line2) =>
			{
				List<Product> products = await db.Products.ToListAsync(), result = new List<Product>();//списки товаров
				if (line1 > 0 && line2 > 0 && line1 < line2
				&& line1 <= products.Count) for (int i = line1 - 1; i < line2 - 1; i++)
					{
						Product product = products[i];//поиск товара
						result.Add(product);
					}
				return result.ToArray();
			});
			app.Run();
		}
	}
}