
using Insurance.Data.Repository.Entities;

namespace Insurance.Data.Repositories
{
    public interface ISurchargeRepository
    {
        Task AddSurchargeRatesAsync(List<SurchargeEntity> surchargeRates);
        Task<decimal> GetSurchargeForProductTypeAsync(int productTypeId);
    }
}


