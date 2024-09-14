namespace Insurance.BusinessLayer.Models
{
    public class ProductTypeModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool CanBeInsured { get; set; }
    }
}
