using Insurance.BusinessLayer.Models;

namespace Insurance.BusinessLayer.Services.Contracts
{
    public interface IInsuranceService
    {
        Task<decimal> CalculateTotalInsuranceForCartAsync(List<InsuranceModel> insuranceRequests);
        Task<InsuranceModel?> CalculateInsuranceAsync(InsuranceModel insuranceRequest, bool forCart = false);
        Task<decimal> CalculateInsuranceValue(InsuranceModel insuranceModel, bool forCart = false);
        Task<ProductModel?> GetProductByIdAsync(int productId);
        Task<ProductTypeModel?> GetProductTypeByIdAsync(int productTypeId);
        decimal ApplyCameraSurcharge(List<InsuranceModel> insuranceRequests, decimal currentTotalInsuranceValue);
        Task<SurchargeModel?> GetSurchargeByProductTypeIdAsync(int productTypeId);
        decimal DetermineBaseInsuranceValue(InsuranceModel insuranceModel);
        int AdditionalInsuranceCost(string productTypeName, bool forCart);
    }
}
