using Microsoft.AspNetCore.Mvc;
using FriendTagBackend.src.Data;
using Microsoft.AspNetCore.Authorization;
using FriendTagBackend.src.Models.User;
using FriendTagBackend.src.Exceptions;
using FriendTagBackend.src.Models.Blocked;
using Microsoft.EntityFrameworkCore;

namespace FriendTagBackend.src.Controllers;

[ApiController]
[Route("[controller]")]
public class BlockController : ControllerBase
{
    private readonly ApiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public BlockController(
        ApiDbContext dbContext,
        IConfiguration configuration,
        UserService userService
    )
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> BlockPerson([FromQuery] Guid personId)
    {
        var user = await _userService.CurrentUser(User);
        var blockedPerson = new UserId(personId);

        if(blockedPerson is null) throw new CustomException("Invalid person.");

        var request = Blocked.Block(user.Id, blockedPerson);

        _dbContext.Blocked.AddAsync(request);
        await _dbContext.SaveChangesAsync();

        return Ok("Person blocked.");
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> UnblockPerson([FromQuery] Guid personId)
    {
        var user = await _userService.CurrentUser(User);
        var blocked = new UserId(personId);

        var block = await _dbContext.Blocked.FirstOrDefaultAsync(x=>x.Blocker == user.Id && x.BlockedPerson == blocked);
        if(block is null) throw new CustomException("This person is not blocked by you.");

        _dbContext.Blocked.Remove(block);
        await _dbContext.SaveChangesAsync();

        return Ok("Person unblocked.");
    }

}