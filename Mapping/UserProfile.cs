using AutoMapper;
using Ecommerce_Api.Models;
using Ecommerce_Api.Models.Dtos;

namespace Ecommerce_Api.Mapping;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserLoginResponseDto>().ReverseMap();

    }
    
    
}