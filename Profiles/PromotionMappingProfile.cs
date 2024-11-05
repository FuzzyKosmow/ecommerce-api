using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_api.DTO.Promotion;
using ecommerce_api.Models;
using AutoMapper;

namespace ecommerce_api.Profiles
{
    public class PromotionMappingProfile : Profile
    {
        public PromotionMappingProfile()
        {
            CreateMap<CreatePromotionDTO, Promotion>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.ValidUntil, opt => opt.MapFrom(src => src.ValidUntil))
                .ForMember(dest => dest.ApplicableProductIds, opt => opt.MapFrom(src => src.ApplicableProductIds));
        }

    }
}