using AutoMapper;
using Insurance.Api.Dtos;
using Insurance.BusinessLayer.Models;
using Insurance.Data.Repository.Entities;

namespace Insurance.Api.Extensions
{
    public class MappingExtensions : Profile
    {
        public MappingExtensions()
        {
            CreateMap<InsuranceDto, InsuranceModel>();
            CreateMap<InsuranceModel, InsuranceDto>();
            CreateMap<ProductSurchargeDto, SurchargeModel>();
            CreateMap<SurchargeModel, SurchargeEntity>();
        }
    }
}
