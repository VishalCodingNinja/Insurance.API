using Insurance.BusinessLayer.Models;

namespace Insurance.BusinessLayer.Services.Contracts
{
    public interface ISurchargeService
    {
        Task UploadSurchargeRatesAsync(List<SurchargeModel> surchargeRates);
        Task<decimal> GetSurchargeForProductTypeAsync(int productTypeId);
    }
}
