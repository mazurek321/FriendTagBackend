using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FriendTagBackend.src.Data;
using FriendTagBackend.src.Exceptions;
using FriendTagBackend.src.Models.User;
using FriendTagBackend.src.Models.Friendship;
using FriendTagBackend.src.Dto.FriendshipDto;

namespace FriendTagBackend.src.Controllers;

[ApiController]
[Route("friends")]
public class FriendShipController : ControllerBase
{
    private readonly ApiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public FriendShipController(
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
    public async Task<IActionResult> AddFriend([FromQuery] Guid UserToSendFriendRequest)
    {
        var requester = await _userService.CurrentUser(User);
        var friendId = new UserId(UserToSendFriendRequest);

        if(friendId is null) throw new CustomException("User not found.");

        if(friendId == requester.Id) throw new CustomException("Invalid id.");

        var exists = await GetFriendship(requester.Id, friendId);
        if(exists is not null) throw new CustomException("Cannot send invitation.");

        var request = Friendship.AddFriend(requester.Id, friendId, requester.Id);

        _dbContext.Friendship.AddAsync(request);
        await _dbContext.SaveChangesAsync();

        return Ok("Friend request sent.");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllFriends()
    {
        var user = await _userService.CurrentUser(User);
        var friends = 
            await _dbContext.Friendship.Where(
                x=>((x.User1 == user.Id)
                ||
                (x.User2 == user.Id))
                && x.Status == Friendship.FStatus.Accepted
                ).ToListAsync();
        return Ok(friends);
    }

    [HttpGet("invites")]
    [Authorize]
    public async Task<IActionResult> GetFriendRequests()
    {
        var user = await _userService.CurrentUser(User);
        var requests = await _dbContext.Friendship.Where(
            x=>
                ((x.User1 == user.Id)
                ||
                (x.User2 == user.Id))
                && x.Status == Friendship.FStatus.Pending
        ).ToListAsync();

        return Ok(requests);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> ChangeFriendshipStatus([FromQuery] Guid userId, UpdateStatusDto updateDto)
    {
        var user = await _userService.CurrentUser(User);
        var friendId = new UserId(userId);

        var friendship = await GetFriendship(user.Id, friendId);

        if(friendship is null) throw new CustomException("Friend not found.");
        if(friendship.RequestSender == user.Id) throw new CustomException("You cannot change the status.");
        
        friendship.SetStatus(updateDto.Status);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteFriend([FromQuery] Guid FriendId)
    {
        var user = await _userService.CurrentUser(User);
        var friendId = new UserId(FriendId);
        
        var exists = await GetFriendship(user.Id, friendId);

        if(exists is null) throw new CustomException("Friend not found.");

        _dbContext.Friendship.Remove(exists);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Friendship> GetFriendship(UserId User1, UserId User2)
    {  
        var exists = 
            await _dbContext.Friendship.FirstOrDefaultAsync(
                x=>
                (x.User1==User1 && x.User2==User2) 
                || 
                (x.User2==User1 && x.User1==User2)
        );   
        
        return exists;
    }
}