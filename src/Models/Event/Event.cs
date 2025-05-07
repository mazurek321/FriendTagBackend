using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Models.Event;

public class Event{
    public Event(){}
    private Event(Guid id, UserId ownerId, String address, String city, 
                double latitude, double longitude, DateOnly date, TimeOnly startTime, TimeOnly endTime,
                bool isPrivate, int? minAge, int? maxAge, List<String>?imageUrls,
                List<String>? tags, String title, String description,
                EventStatus status
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
        Attendants = new List<EventAttendee>();
        Title = title;
        Description = description;
        Status = status;
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
    public int? MinAge { get; private set; } 
    public int? MaxAge { get; private set; }
    public List<String>?ImageUrls { get; private set; } 
    public List<String>? Tags { get; private set; }
    public List<EventAttendee>? Attendants { get; private set; } = new List<EventAttendee>();
    public String Title { get; private set; }
    public String Description { get; private set; }
    public EventStatus Status { get; private set; }

    public enum EventStatus{
        Planned,
        Completed,
        Canceled
    }

    public static Event NewEvent(
        UserId ownerId, String address, String city, 
        double latitude, double longitude, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        bool isPrivate, int? minAge, int? maxAge, List<String>?imageUrls,
        List<String>? tags, String title, String description
    )
    {
        return new Event(
            Guid.NewGuid(), ownerId, address, city, latitude, longitude, 
            date, startTime, endTime, isPrivate, minAge, maxAge, 
            imageUrls, tags, title, 
            description, EventStatus.Planned
        );
    }

    public void UpdateEvent(string address,
        string city,
        double latitude,
        double longitude,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isPrivate,
        int? minAge,
        int? maxAge,
        List<string>? imageUrls,
        List<string>? tags,
        string title,
        string description)
    {
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
        ImageUrls = imageUrls;
        Tags = tags;
        Title = title;
        Description = description;
    }

    public void UpdateAttentands(List<EventAttendee> attendants)
    {
        Attendants = attendants;
    }

    public void AddAttendant(User.User user)
    {
        Attendants.Add(new EventAttendee(this.Id, this, user.Id, user));
    }


    public void RemoveAttendant(User.User user)
    {
        var attendeeToRemove = Attendants.FirstOrDefault(a => a.UserId == user.Id);
        if (attendeeToRemove != null)
        {
            Attendants.Remove(attendeeToRemove);
        }
    }



}
