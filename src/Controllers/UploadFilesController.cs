using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using FriendTagBackend.src.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using FriendTagBackend.src.Models.Message;
using FriendTagBackend.src.Models.User;
using Microsoft.EntityFrameworkCore;

namespace FriendTagBackend.src.Controllers;

[ApiController]
[Route("[controller]")]
[EnableCors("AllowAnyOrigin")]
public class UploadFilesController : ControllerBase
{
    private readonly ApiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;
    private readonly IWebHostEnvironment _env;

    public UploadFilesController(
        ApiDbContext dbContext,
        IConfiguration configuration,
        UserService userService,
        IWebHostEnvironment env
    )
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _userService = userService;
        _env = env;
    }

    [HttpPost("upload/user")]
    [Authorize]
    public async Task<IActionResult> UploadUserPicture([FromForm] UploadUserProfileRequest request)
    {
        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        if (string.IsNullOrEmpty(webRootPath))
            return StatusCode(500, "Server misconfiguration: WebRootPath is not set.");

        var file = request.File;
        if (file == null || file.Length == 0)
            return BadRequest("Brak pliku");

        var user = await _userService.CurrentUser(User);
        var userId = user.Id.Value.ToString();

        var userFolder = Path.Combine(webRootPath, "uploads", "users", userId);
        Directory.CreateDirectory(userFolder);

        var fileName = "profile" + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine("FILENAME: " + fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"http://localhost:5275/uploads/users/{userId}/{fileName}";

        user.UpdateProfilePicture(fileUrl);
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new { url = fileUrl });
    }

    [HttpPost("upload/event")]
    [Authorize]
    public async Task<IActionResult> UploadEventPhoto([FromQuery] Guid eventId, [FromForm] UploadEventPhotosRequest request)
    {
        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var files = request.Files;
        if (files == null || files.Count == 0)
            return BadRequest("Brak plików");

        var uploadsPath = Path.Combine(webRootPath, "uploads", "events", eventId.ToString());
        Directory.CreateDirectory(uploadsPath);

        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            var uniqueName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"http://localhost:5275/uploads/events/{eventId}/{uniqueName}";
            uploadedUrls.Add(fileUrl);
        }

        return Ok(new { urls = uploadedUrls });
    }

    [HttpPost("upload/message")]
    [Authorize]
    public async Task<IActionResult> SendMessage(
        [FromQuery] Guid userId,
        [FromForm] UploadImageMessageDto dto)  
    {
        var user = await _userService.CurrentUser(User);
        var receiverId = new UserId(userId);
        var receiver = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == receiverId);
        if (receiver == null)
            return NotFound("Receiver user not found.");

        string? imageUrl = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "messages");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imageUrl = $"http://localhost:5275/uploads/messages/{fileName}";
        }

        var message = Message.NewMessage(
            user.Id,
            receiverId,
            dto.Content ?? "",
            DateTime.Now,
            imageUrl ?? string.Empty, 
            user,
            receiver
        );

        await _dbContext.Messages.AddAsync(message);
        await _dbContext.SaveChangesAsync();

        return Ok(message);
    }



}
