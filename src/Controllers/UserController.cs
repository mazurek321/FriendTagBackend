using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FriendTagBackend.src.Controllers.Dto.UserDto;
using FriendTagBackend.src.Data;
using FriendTagBackend.src.Exceptions;
using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Controllers;

[ApiController]
[Route("[controller]")]
[EnableCors("AllowAnyOrigin")]
public class UsersController : ControllerBase
{
    private readonly ApiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public UsersController(
        ApiDbContext dbContext,
        IConfiguration configuration,
        UserService userService
    )
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("register", Name = "Create new user")]
    public async Task<IActionResult> Signup([FromBody] RegisterDto userDto)
    {
        var email = new Email(userDto.Email);
        var name = new Name(userDto.Name);
        var lastname = new LastName(userDto.Lastname);
        var password = new Password(userDto.Password);
        var confirmPassword = new Password(userDto.ConfirmPassword);
        var latitude = double.TryParse(userDto.Latitude, out var lat) ? (double?)lat : null;
        var longitude = double.TryParse(userDto.Longitude, out var lon) ? (double?)lon : null;

        var exists = await _dbContext.Users.AnyAsync(x => x.Email == email);
        if (exists) throw new CustomException("User with this email already exists.");

        var hashedPassword = new Password(password.CalculateMD5Hash(password.Value));

        var user = Models.User.User.NewUser(
            email,
            name,
            lastname,
            hashedPassword,
            userDto.Birthday,
            latitude.Value,
            longitude.Value,
            userDto?.ProfilePicture,
            userDto?.Description,
            userDto.Phone,
            DateTime.UtcNow
        );

        _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> Login(LoginDto userDto)
    {
        var email = new Email(userDto.Email);
        var password = new Password(userDto.Password);

        var hashedPassword = new Password(password.CalculateMD5Hash(password.Value.Trim()));

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email && x.Password == hashedPassword);

        if (user != null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.Value.ToString()),
                new Claim("Email", user.Email.Value.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.MaxValue,
                signingCredentials: signIn
            );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(tokenValue);
        }
        return Unauthorized();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var user = await _userService.CurrentUser(User);
        return Ok(user);
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetById([FromQuery] Guid userId)
    {
        var currentUser = await _userService.CurrentUser(User);
        var targetUserId = new UserId(userId);

        var isBlocked = await _dbContext.Blocked.AnyAsync(b =>
            (b.Blocker == currentUser.Id && b.BlockedPerson == targetUserId) ||
            (b.Blocker == targetUserId && b.BlockedPerson == currentUser.Id));

        if (isBlocked)
            return Forbid("You are blocked or have blocked this user.");

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == targetUserId);
        if (user is null) throw new CustomException("User not found.");

        return Ok(user);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        var currentUser = await _userService.CurrentUser(User);
        var currentUserId = currentUser.Id;

        var blockedUsers = await _dbContext.Blocked
            .Where(b => b.Blocker == currentUserId || b.BlockedPerson == currentUserId)
            .ToListAsync();

        var usersBlockingMe = blockedUsers
            .Where(b => b.BlockedPerson == currentUserId)
            .Select(b => b.Blocker)
            .ToHashSet();

        var usersBlockedByMe = blockedUsers
            .Where(b => b.Blocker == currentUserId)
            .Select(b => b.BlockedPerson)
            .ToHashSet();

        var allUsers = await _dbContext.Users
            .Where(u => !usersBlockingMe.Contains(u.Id) && !usersBlockedByMe.Contains(u.Id) && u.Id != currentUserId)
            .ToListAsync();

        return Ok(allUsers);
    }

    [HttpPut("picture")]
    [Authorize]
    public async Task<IActionResult> UpdateProfilePicture([FromQuery] String url)
    {
        if (url is null) throw new CustomException("Image is null.");

        var currentUser = await _userService.CurrentUser(User);

        currentUser.UpdateProfilePicture(url);

        return Ok();

    }
    

    [HttpPut("location")]
    [Authorize]
    public async Task<IActionResult> UpdateLocation([FromQuery] double latitude, double longitude)
    {
        if (latitude == null || longitude == null) throw new CustomException("Error.");

        var currentUser = await _userService.CurrentUser(User);

        currentUser.UpdateLocation(latitude, longitude);

        return Ok();

    }


}


