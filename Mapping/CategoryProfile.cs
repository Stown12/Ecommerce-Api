using AutoMapper;
using Ecommerce_Api.Models;
using Ecommerce_Api.Models.Dtos;

namespace Ecommerce_Api.Mapping;

public class CategoryProfile: Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CreateCategoryDto>().ReverseMap();
    }
}