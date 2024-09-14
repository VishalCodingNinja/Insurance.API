using Microsoft.AspNetCore.Http;
using System;

namespace Insurance.Api.Helpers
{
    public class BaseAddressProvider : IBaseAddressProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseAddressProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetBaseAddress()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("No HTTP context available.");
            }

            // Construct the base address
            var baseAddress = $"{request.Scheme}://{request.Host}";
            return baseAddress;
        }
    }

}
