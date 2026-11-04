using Ecommerce_Api.Models;
using Ecommerce_Api.Models.Dtos;

namespace Ecommerce_Api.Repository.IRepository;

public interface IUserRepository
{
    public ICollection<User> GetUsers();
    public User? GetUser(int id);
    public bool IsUniqueUser(string username);
    public Task<UserLoginResponseDto?> Login(UserLoginDto userLoginDto);
    public Task<User?> Register(CreateUserDto createUserDto);
}