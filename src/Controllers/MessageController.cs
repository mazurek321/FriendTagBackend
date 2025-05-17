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
        public async Task<IActionResult> SendMessage(
                [FromQuery] Guid userId,
                [FromBody] SendMessageDto dto)
        {
            var user = await _userService.CurrentUser(User);
            var receiverId = new UserId(userId);
            var receiver = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id == receiverId);

            var message = Message.NewMessage(
                user.Id,
                receiverId,
                dto.Content,
                DateTime.Now,
                dto.Image,
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
        public async Task<IActionResult> GetConversations()
        {
            var user = await _userService.CurrentUser(User);
            var usersIds = await _dbContext.Messages
                                    .Where(x => x.SenderId == user.Id || x.ReceiverId == user.Id)
                                    .Select(x=>x.SenderId == user.Id ? x.ReceiverId : x.SenderId) 
                                    .Distinct()
                                    .ToListAsync();
            return Ok(usersIds);
        }

        [HttpGet("conversation")]
        [Authorize]
        public async Task<IActionResult> GetMessages([FromQuery] Guid userId)
        {
            var user1 = await _userService.CurrentUser(User);
            var user1Id = user1.Id;
            var user2Id = new UserId(userId);
            var messages = await _dbContext.Messages
                .Where(m =>
                    (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                    (m.SenderId == user2Id && m.ReceiverId == user1Id)
                )
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }

    }

    
}