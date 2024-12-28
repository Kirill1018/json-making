using System.ComponentModel.DataAnnotations;

namespace JsonMaking
{
	public class Product
	{
		[Key] public int Id { get; set; }
		[Required] public string Name { get; set; } = "";//название товара
		public int Price { get; set; }
		public Product() { }
		public override string? ToString() { return this.Id + ", " + this.Name +
				", " + this.Price; }
	}
}