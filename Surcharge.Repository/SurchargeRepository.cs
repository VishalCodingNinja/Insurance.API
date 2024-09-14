using Insurance.Data.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Data.Repositories
{
    /// <summary>
    /// Repository class for managing surcharge-related database operations.
    /// </summary>
    public class SurchargeRepository : ISurchargeRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor to initialize the surcharge repository with the database context.
        /// </summary>
        /// <param name="context">The database context.</param>
        public SurchargeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds or updates surcharge rates for different product types.
        /// </summary>
        /// <param name="surchargeRates">List of surcharge rate models containing ProductTypeId and Surcharge.</param>
        public async Task AddSurchargeRatesAsync(List<SurchargeEntity> surchargeRates)
        {
            // Fetch all existing surcharges from the database
            var existingSurcharges = await _context.Surcharges.ToListAsync();

            // Create a dictionary from the existing surcharges for easier access
            var existingSurchargeDict = existingSurcharges.ToDictionary(s => s.ProductTypeId);

            foreach (var rate in surchargeRates)
            {
                // Check if the product type already exists
                if (existingSurchargeDict.TryGetValue(rate.ProductTypeId, out var existingSurcharge))
                {
                    // Update the existing surcharge if found
                    existingSurcharge.Surcharge = rate.Surcharge;
                    _context.Surcharges.Update(existingSurcharge);
                }
                else
                {
                    // Create a new surcharge entry if it doesn't exist
                    var newSurcharge = new SurchargeEntity
                    {
                        ProductTypeId = rate.ProductTypeId,
                        Surcharge = rate.Surcharge
                    };
                    await _context.Surcharges.AddAsync(newSurcharge);
                }
            }

            // Save all changes in the database
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the surcharge for a specific product type by its ID.
        /// </summary>
        /// <param name="productTypeId">The ID of the product type.</param>
        /// <returns>The surcharge for the specified product type.</returns>
        public async Task<decimal> GetSurchargeForProductTypeAsync(int productTypeId)
        {
            return await _context.Surcharges
                .Where(s => s.ProductTypeId == productTypeId)
                .Select(s => s.Surcharge)
                .FirstOrDefaultAsync();
        }
    }
}
