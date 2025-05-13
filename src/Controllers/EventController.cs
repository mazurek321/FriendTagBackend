using FriendTagBackend.src.Data;
using FriendTagBackend.src.Dto.EventDto;
using FriendTagBackend.src.Exceptions;
using FriendTagBackend.src.Models.Event;
using FriendTagBackend.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FriendTagBackend.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public EventController(ApiDbContext dbContext, IConfiguration configuration, UserService userService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent(EventDto eventDto)
        {
            var user = await _userService.CurrentUser(User);

            var request = Event.NewEvent(
                user.Id,
                eventDto.Address,
                eventDto.City,
                eventDto.Latitude,
                eventDto.Longitude,
                eventDto.Date,
                eventDto.StartTime,
                eventDto.EndTime,
                eventDto.Private,
                eventDto.MinAge,
                eventDto.MaxAge,
                eventDto.ImageUrls,
                eventDto.Tags,
                eventDto.Title,
                eventDto.Description
            );

            await _dbContext.Events.AddAsync(request);
            await _dbContext.SaveChangesAsync();

            var response = ToGetEventDto(request, true);
            return Ok(response);
        }

        [HttpPost("join")]
        [Authorize]
        public async Task<IActionResult> JoinEvent([FromQuery] Guid eventId)
        {
            var user = await _userService.CurrentUser(User);
            var _event = await _dbContext.Events.Include(e => e.Attendants).FirstOrDefaultAsync(e => e.Id == eventId);

            if (_event is null) throw new CustomException("Event not found.");
            if (_event.Private) throw new CustomException("This event is private. Ask owner for joining.");

            _event.AddAttendant(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Joined event.");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetEvents([FromQuery] Guid? eventId)
        {
            var user = await _userService.CurrentUser(User);
            var userId = user.Id;

            var blockedUsers = await _dbContext.Blocked
                .Where(b => b.Blocker == userId || b.BlockedPerson == userId)
                .ToListAsync();

            var usersBlockingMe = blockedUsers.Where(b => b.BlockedPerson == userId).Select(b => b.Blocker).ToHashSet();
            var usersBlockedByMe = blockedUsers.Where(b => b.Blocker == userId).Select(b => b.BlockedPerson).ToHashSet();

            if (eventId.HasValue)
            {
                var e = await _dbContext.Events.Include(x => x.Attendants).FirstOrDefaultAsync(x => x.Id == eventId);

                if (e == null) throw new CustomException("Event not found.");
                if (usersBlockingMe.Contains(e.OwnerId) || usersBlockedByMe.Contains(e.OwnerId)) return Ok(null);

                 var attendants = e.Attendants ?? new List<EventAttendee>();
                bool isAttendant = attendants.Any(a => a.UserId == user.Id) || e.OwnerId == user.Id;
                return Ok(ToGetEventDto(e, isAttendant));
            }

            var events = await _dbContext.Events
                .Include(x => x.Attendants)
                .Where(e => !usersBlockingMe.Contains(e.OwnerId) && !usersBlockedByMe.Contains(e.OwnerId))
                .ToListAsync();

            var eventDtos = events.Select(e =>
            {
                var attendants = e.Attendants ?? new List<EventAttendee>();
                bool isAttendant = attendants.Any(a => a.UserId == user.Id) || e.OwnerId == user.Id;
                return ToGetEventDto(e, isAttendant);
            }).ToList();

            return Ok(eventDtos);
        }

        [HttpGet("attending")]
        [Authorize]
        public async Task<IActionResult> GetAttendingEvents()
        {
            var user = await _userService.CurrentUser(User);
            var userId = user.Id;

            var blockedUsers = await _dbContext.Blocked
                .Where(b => b.Blocker == userId || b.BlockedPerson == userId)
                .ToListAsync();

            var usersBlockingMe = blockedUsers.Where(b => b.BlockedPerson == userId).Select(b => b.Blocker).ToHashSet();
            var usersBlockedByMe = blockedUsers.Where(b => b.Blocker == userId).Select(b => b.BlockedPerson).ToHashSet();

            var events = await _dbContext.Events.Include(e => e.Attendants)
                .Where(e => e.Attendants.Any(a => a.UserId == userId) &&
                            !usersBlockingMe.Contains(e.OwnerId) &&
                            !usersBlockedByMe.Contains(e.OwnerId))
                .ToListAsync();

            var eventDtos = events.Select(e => ToGetEventDto(e, true)).ToList();
            return Ok(eventDtos);
        }

        [HttpGet("myEvents")]
        [Authorize]
        public async Task<IActionResult> GetMyEvents()
        {
            var user = await _userService.CurrentUser(User);
            var events = await _dbContext.Events
                .Where(e => e.OwnerId == user.Id)
                .ToListAsync();

            var eventDtos = events.Select(e => ToGetEventDto(e, true)).ToList();
            return Ok(eventDtos);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateEvent([FromQuery] Guid eventId, UpdateEventDto dto)
        {
            var user = await _userService.CurrentUser(User);
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            if (e is null) throw new CustomException("Event not found.");
            if (e.OwnerId != user.Id) throw new CustomException("You can't update the event.");

            e.UpdateEvent(
                dto.Address,
                dto.City,
                dto.Latitude,
                dto.Longitude,
                dto.Date,
                dto.StartTime,
                dto.EndTime,
                dto.Private,
                dto.MinAge,
                dto.MaxAge,
                dto.ImageUrls,
                dto.Tags,
                dto.Title,
                dto.Description
            );

            await _dbContext.SaveChangesAsync();
            return Ok(ToGetEventDto(e, true));
        }

        [HttpPut("attendants")]
        [Authorize]
        public async Task<IActionResult> UpdateAttendants([FromQuery] Guid eventId, [FromBody] List<EventAttendee> attendants)
        {
            var user = await _userService.CurrentUser(User);
            var e = await _dbContext.Events.Include(x => x.Attendants).FirstOrDefaultAsync(x => x.Id == eventId);

            if (e is null) throw new CustomException("Event not found.");
            if (e.OwnerId != user.Id) throw new CustomException("You can't update the event.");

            e.UpdateAttentands(attendants);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteEvent([FromQuery] Guid eventId)
        {
            var user = await _userService.CurrentUser(User);
            var e = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            if (e is null || e.OwnerId != user.Id) throw new CustomException("You can't delete this event.");

            _dbContext.Events.Remove(e);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("leave")]
        [Authorize]
        public async Task<IActionResult> LeaveEvent([FromQuery] Guid eventId)
        {
            var user = await _userService.CurrentUser(User);
            var e = await _dbContext.Events.Include(x => x.Attendants).FirstOrDefaultAsync(x => x.Id == eventId);
            if (e is null) throw new CustomException("Event not found.");

            var guest = e.Attendants.FirstOrDefault(a => a.UserId == user.Id);
            if (guest is null) throw new CustomException("You are not a guest in this event.");

            e.RemoveAttendant(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        private GetEventDto ToGetEventDto(Event e, bool isAttendant)
        {
            return new GetEventDto
            {
                Id = e.Id,
                OwnerId = e.OwnerId.Value,
                Address = (e.Private && !isAttendant) ? null : e.Address,
                City = (e.Private && !isAttendant) ? null : e.City,
                Latitude = (e.Private && !isAttendant) ? 0 : e.Latitude,
                Longitude = (e.Private && !isAttendant) ? 0 : e.Longitude,
                Date = (e.Private && !isAttendant) ? DateOnly.MinValue : e.Date,
                StartTime = (e.Private && !isAttendant) ? TimeOnly.MinValue : e.StartTime,
                EndTime = (e.Private && !isAttendant) ? TimeOnly.MinValue : e.EndTime,
                Private = e.Private,
                MinAge = e.MinAge,
                MaxAge = e.MaxAge,
                ImageUrls = e.ImageUrls,
                Tags = e.Tags,
                Title = e.Title,
                Description = e.Description,
                Status = e.Status
            };
        }
    }
}
