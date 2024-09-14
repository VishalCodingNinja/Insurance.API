namespace Insurance.BusinessLayer.Models
{
    public class InsuranceModel
    {
        public int ProductId { get; set; }
        public decimal InsuranceValue { get; set; }
        public ProductModel? ProductModel { get; set; } = null;

    }
}
