using AutoMapper;
using Ecommerce_Api.Models.Dtos;
using Ecommerce_Api.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Ecommerce_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status403Forbidden)]
    public IActionResult GetUsers(CancellationToken ct)
    {
        var users = _userRepository.GetUsers();
        var usersDto = _mapper.Map<List<UserDto>>(users);
        return Ok(usersDto);
    }

    [HttpGet("{Id:int}", Name = "GetUser")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status403Forbidden)]
    [ProducesResponseType(Status400BadRequest)]
    public IActionResult GetUserById(int id, CancellationToken ct)
    {
        var user = _userRepository.GetUser(id);
        if (user == null) return NotFound($"User with id: {id} not found");
        
        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    [HttpPost(Name = "RegisterUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
    {
        if(createUserDto == null || !ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(createUserDto.Name)) return BadRequest("UserName is required");
        if(!_userRepository.IsUniqueUser(createUserDto.Username)) return BadRequest("User already exists");
        
        var result = await _userRepository.Register(createUserDto);
        if(result == null) return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong while registering the user");
        return CreatedAtRoute("GetUser", new { id = result.Id }, result);
    }

    [HttpPost("Login", Name = "LoginUser")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
    {
        if(userLoginDto == null || !ModelState.IsValid) return BadRequest(ModelState);
        
        var user = await _userRepository.Login(userLoginDto);
        if (user == null) return Unauthorized();
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        return NoContent();
    }
}