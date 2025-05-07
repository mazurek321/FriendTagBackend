using FriendTagBackend.src.Data;
using FriendTagBackend.src.Exceptions;
using FriendTagBackend.src.Models.Event;
using FriendTagBackend.src.Models.Invitation;
using FriendTagBackend.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendTagBackend.src.Controllers;

[ApiController]
[Route("[controller]")]
public class InvitationController : ControllerBase
{
    private readonly ApiDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public InvitationController(
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
    public async Task<IActionResult> SendInvitation(
        [FromQuery] Guid eventId, 
        [FromQuery] Guid userId)
    {
         var user = await _userService.CurrentUser(User);
         var _event = await _dbContext.Events.FirstOrDefaultAsync(x=>x.Id == eventId);

         if(_event is null) throw new CustomException("Event not found.");
         var guest = new UserId(userId);

         if(_event.OwnerId != user.Id) throw new CustomException("You can't invite people.");

         var request = Invitation.SendInvitation(eventId, guest);

         _dbContext.Invitations.AddAsync(request);
         await _dbContext.SaveChangesAsync();

        return Ok(request);
    }

    [HttpPost("request")]
    [Authorize]
    public async Task<IActionResult> RequestAnInvitation([FromQuery] Guid eventId)
    {
        var user = await _userService.CurrentUser(User);
        var _event = await _dbContext.Events.FirstOrDefaultAsync(x=>x.Id == eventId);

        if(_event is null) throw new CustomException("Event not found.");
        if(!_event.Private) throw new CustomException("Event is not private.");

        var exists = await _dbContext.Invitations.AnyAsync(x=>x.EventId == _event.Id && x.InvitedPerson == user.Id);
        if(exists) throw new CustomException("You already send a request to join this event.");

        var request = Invitation.RequestInvitation(eventId, user.Id);

        _dbContext.Invitations.AddAsync(request);
         await _dbContext.SaveChangesAsync();

        return Ok(request);
    }

    [HttpPut("respond")]
    [Authorize]
    public async Task<IActionResult> RespondToInvitation(
        [FromQuery] Guid Id,
        [FromBody] Invitation.InvitationStatus status)
    {
        var currentUser = await _userService.CurrentUser(User);
        
        var invitation = await _dbContext.Invitations
            .FirstOrDefaultAsync(x => x.Id == Id && x.InvitedPerson == currentUser.Id);

        if (invitation == null) throw new CustomException("Invitation not found.");
        if (invitation.Status != Invitation.InvitationStatus.Sent)
            throw new CustomException("Invalid invitation status for response.");

        invitation.UpdateInvitation(status);

        if (status == Invitation.InvitationStatus.Accepted)
        {
            var _event = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == invitation.EventId);
            _event?.AddAttendant(currentUser);
        }

        await _dbContext.SaveChangesAsync();
        return Ok(invitation);
    }

    [HttpPut("respond-to-request")]
    [Authorize]
    public async Task<IActionResult> RespondToInvitationRequest(
        [FromQuery] Guid id,
        [FromBody] Invitation.InvitationStatus status)
    {
        var currentUser = await _userService.CurrentUser(User);
        var invitation = await _dbContext.Invitations.FirstOrDefaultAsync(x=>x.Id == id);
        var guestAttendant = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id == invitation.InvitedPerson);

        var _event = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == invitation.EventId);
        if (_event == null) throw new CustomException("Event not found.");
        if (_event.OwnerId != currentUser.Id)
            throw new CustomException("Only the owner can respond to invitation requests.");

        var _invitation = await _dbContext.Invitations
            .FirstOrDefaultAsync(x => x.EventId == invitation.EventId && x.InvitedPerson == guestAttendant.Id && x.Status == Invitation.InvitationStatus.Request);

        if (invitation == null) throw new CustomException("Invitation request not found.");

        _invitation.UpdateInvitation(status);

        if (status == Invitation.InvitationStatus.Accepted)
        {
            _event.AddAttendant(guestAttendant);
        }

        await _dbContext.SaveChangesAsync();
        return Ok(invitation);
    }


    [HttpGet("requests")]
    [Authorize]
    public async Task<IActionResult> GetRequestsToMyEvents()
    {
        var user = await _userService.CurrentUser(User);

        var requests = await _dbContext.Invitations
            .Where(i => i.Status == Invitation.InvitationStatus.Request)
            .Join(_dbContext.Events,
                invitation => invitation.EventId,
                ev => ev.Id,
                (invitation, ev) => new { Invitation = invitation, Event = ev })
            .Where(x => x.Event.OwnerId == user.Id)
            .Select(x => x.Invitation)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("my-invitations")]
    [Authorize]
    public async Task<IActionResult> GetInvitationsSentToMe()
    {
        var currentUser = await _userService.CurrentUser(User);

        var invitations = await _dbContext.Invitations
            .Where(i => i.InvitedPerson == currentUser.Id && i.Status == Invitation.InvitationStatus.Sent)
            .ToListAsync();

        return Ok(invitations);
    }

    

}
