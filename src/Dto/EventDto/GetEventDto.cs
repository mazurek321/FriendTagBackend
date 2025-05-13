using FriendTagBackend.src.Models.Event;

namespace FriendTagBackend.src.Dto.EventDto;

public record GetEventDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool Private { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public List<string>? ImageUrls { get; set; }
    public List<string>? Tags { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Event.EventStatus Status { get; set; }

}