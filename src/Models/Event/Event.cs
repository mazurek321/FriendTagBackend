using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Models.Event;

public class Event{
    public Event(){}
    private Event(Guid id, UserId ownerId, String address, String city, 
                double latitude, double longitude, DateOnly date, TimeOnly startTime, TimeOnly endTime,
                bool isPrivate, int minAge, int maxAge, List<String>?imageUrls,
                List<String>? tags, List<UserId>? attendants, String title, String description,
                EventStatus status, bool allowComments
                )
    {
        Id = id;
        OwnerId = ownerId;
        Address = address;
        City = city;
        Latitude = latitude;
        Longitude = longitude;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Private = isPrivate;
        MinAge = minAge;
        MaxAge = maxAge;
        Tags = tags;
        Attendants = attendants;
        Title = title;
        Description = description;
        Status = status;
        AllowComments = allowComments;
    }
    public Guid Id { get; private set; }
    public UserId OwnerId { get; private set; }
    public String Address { get; private set; }
    public String City { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool Private { get; private set; }
    public int MinAge { get; private set; } 
    public int MaxAge { get; private set; }
    public List<String>?ImageUrls { get; private set; } 
    public List<String>? Tags { get; private set; }
    public List<UserId>? Attendants { get; private set; }
    public String Title { get; private set; }
    public String Description { get; private set; }
    public EventStatus Status { get; private set; }
    public bool AllowComments { get; private set; }

    public enum EventStatus{
        Planned,
        Completed,
        Canceled
    }

    public static Event NewEvent(
        UserId ownerId, String address, String city, 
        double latitude, double longitude, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        bool isPrivate, int minAge, int maxAge, List<String>?imageUrls,
        List<String>? tags, List<UserId>? attendants, String title, String description,
        EventStatus status, bool allowComments
    )
    {
        return new Event(
            Guid.NewGuid(), ownerId, address, city, latitude, longitude, 
            date, startTime, endTime, isPrivate, minAge, maxAge, 
            imageUrls, tags, attendants, title, 
            description, status, allowComments
        );
    }


}
