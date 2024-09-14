namespace Insurance.BusinessLayer.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal SalesPrice { get; set; }
        public int ProductTypeId { get; set; }
        public ProductTypeModel? ProductType { get; set; }
    }
}
