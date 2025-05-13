using FriendTagBackend.src.Data;
using FriendTagBackend.src.Models.Message;
using FriendTagBackend.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendTagBackend.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public MessageController(ApiDbContext dbContext, IConfiguration configuration, UserService userService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromQuery] Guid userId, SendMessageDto dto)
        {
            var user = await _userService.CurrentUser(User);
            var receiverId = new UserId(userId);
            var receiver = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id == receiverId);

            // var file = await SaveFileAsync(dto.Image);
            var message = Message.NewMessage(
                user.Id,
                receiverId,
                dto.Content,
                DateTime.Now,
                // dto.Image,
                dto.Latitude,
                dto.Longitude,
                user,
                receiver
            );

            _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();

            return Ok(message);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMessages([FromQuery] Guid userId)
        {
            var user = await _userService.CurrentUser(User);
            var messagerId = new UserId(userId);
           
            var messages = await _dbContext.Messages.Where(x=>x.ReceiverId == messagerId).ToListAsync();
           
            return Ok(messages);
        }


        // public async Task<string> SaveFileAsync(IFormFile file)
        // {
        //     if (file == null || file.Length == 0)
        //     {
        //         throw new ArgumentException("No file provided.");
        //     }

        //     var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);

        //     using (var fileStream = new FileStream(filePath, FileMode.Create))
        //     {
        //         await file.CopyToAsync(fileStream);
        //     }

        //     return filePath; 
        // }

    }

    
}