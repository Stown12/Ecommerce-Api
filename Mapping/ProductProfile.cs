using AutoMapper;
using Ecommerce_Api.Models;
using Ecommerce_Api.Models.Dtos;

namespace Ecommerce_Api.Mapping;

public class ProductProfile: Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }
}