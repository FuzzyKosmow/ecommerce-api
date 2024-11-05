using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ecommerce_api.DTO.Category;
using ecommerce_api.DTO.Product;

using ecommerce_api.Models;
namespace ecommerce_api.Profiles
{
    public class ProductMappingProfile : Profile
    {
        // Product -> ProductDetailsDTO

        public ProductMappingProfile()
        {
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Product, ProductDetailsDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount_price, opt => opt.MapFrom(src => src.DiscountPrice))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors))
                .ForMember(dest => dest.Storage, opt => opt.MapFrom(src => src.StorageOptions))
                .ForMember(dest => dest.Specifications, opt => opt.MapFrom(src => src.Specifications))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                src.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()))
                .ForMember(dest => dest.Is_bestseller, opt => opt.MapFrom(src => src.IsBestSeller))
                .ForMember(dest => dest.Is_featured, opt => opt.MapFrom(src => src.IsFeatured))
                .ForMember(dest => dest.Is_new_arrival, opt => opt.MapFrom(src => src.IsNewArrival))
                .ForMember(dest => dest.Release_date, opt => opt.MapFrom(src => src.ReleaseDate));


            // Product -> ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount_price, opt => opt.MapFrom(src => src.DiscountPrice))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Images.FirstOrDefault()))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors))
                .ForMember(dest => dest.Storage, opt => opt.MapFrom(src => src.StorageOptions))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                src.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()))

                .ForMember(dest => dest.Is_bestseller, opt => opt.MapFrom(src => src.IsBestSeller))
                .ForMember(dest => dest.Is_featured, opt => opt.MapFrom(src => src.IsFeatured))
                .ForMember(dest => dest.Is_new_arrival, opt => opt.MapFrom(src => src.IsNewArrival))
                .ForMember(dest => dest.Release_date, opt => opt.MapFrom(src => src.ReleaseDate));
        }
    }
}