using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce_Api.Data;
using Ecommerce_Api.Models;
using Ecommerce_Api.Models.Dtos;
using Ecommerce_Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce_Api.Repository;

public class UserRepository:IUserRepository
{
    public readonly ApplicationDbContext _dbContext;
    // Propiedad para obtener el secret_key de appsettings.json
    private string? secretKey;
    
    public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        secretKey = configuration["ApiSettings:SecretKey"];
    }
    
    
    public ICollection<User> GetUsers()
    {
        return _dbContext.Users.OrderBy(u => u.UserName).ToList();
    }

    public User? GetUser(int id)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == id);
        
    }

    public bool IsUniqueUser(string username)
    {
        //* quitamos el operador ? para que permita usar los metodos tolower y trim, ya que el operador ? indica que puede ser nulo y no permite usar esos metodos
        return !_dbContext.Users.Any(u => u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.Username))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username is required"
            };
        }
        
        var user = await _dbContext.Users.FirstOrDefaultAsync<User>(u => u.UserName.ToLower().Trim() == userLoginDto.Username.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "User not found"
            };
        }
        
        if(!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Credentials are incorrect"
            };
        }

        // Generacion de JWT
        var handlerToken = new JwtSecurityTokenHandler();
        if(string.IsNullOrWhiteSpace(secretKey)) throw new InvalidOperationException("Secret key is not configured.");
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto()
            {
                Username = user.UserName,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password ?? ""
            },
            Message = "Login successful"
        };
    }

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        // Encriptacion de password con bcryps
        var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User()
        {
            Name = createUserDto.Name,
            UserName = createUserDto.Username ?? "No username",
            Password = encriptedPassword,
            Role = createUserDto.Role
        };

        _dbContext.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}