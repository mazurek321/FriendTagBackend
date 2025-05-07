using System.Text.Json.Serialization;
using FriendTagBackend.src.Models.Event;
using FriendTagBackend.src.Models.User;

public class EventAttendee
{
    public EventAttendee() { }

    public EventAttendee(Guid eventId, Event _event, UserId userId, User user)
    {
        EventId = eventId;
        Event = _event;
        UserId = userId;
        User = user;
    }
    public Guid EventId { get; set; }
    [JsonIgnore]
    public Event Event { get; set; }
    public UserId UserId { get; set; }
    public User User { get; set; }
}
