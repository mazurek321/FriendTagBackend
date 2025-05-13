namespace FriendTagBackend.src.Models.User;

public class User
{     
    public User(){}
    
    private User(
        UserId id, Email email, Name name, LastName lastname, Password password,
        DateOnly birthday, 
        double latitude, double longitude, String? profilePicture, String? desc,
        Phone phone, DateTime createdAt
    ){
        Id = id;
        Email = email;
        Name = name;
        LastName = lastname;
        Password = password;
        Birthday = birthday;
        Latitude = latitude;
        Longitude = longitude;
        ProfilePicture = profilePicture;
        Description = desc;
        Phone = phone;
        CreatedAt = createdAt;
    }

    public UserId Id {get; private set; }
    public Email Email { get; private set; }
    public Name Name {get; private set; }
    public LastName LastName {get; private set; }
    public Password Password {get; private set; }
    public Phone Phone { get; private set; }
    public DateOnly Birthday { get; private set; }
    public double Latitude {get; private set;} 
    public double Longitude {get; private set;} 
    public String? ProfilePicture {get; private set;}
    public List<String>?ImageUrls { get; private set; } 
    public String? Description {get; private set; }
    public DateTime CreatedAt{ get; private set; }

    public static User NewUser(
        Email email, Name name, LastName lastname, Password password,
        DateOnly birthday,
        double latitude, double longitude, String? profilePicture, String? desc,
        Phone phone, DateTime createdAt
    ){
        var id = new UserId(Guid.NewGuid());
        return new User(id, email, name, lastname, password, birthday, latitude, 
                        longitude, profilePicture, desc, phone, createdAt);
    }

    public void UpdateLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public void UpdateProfilePicture(string profilePicture)
    {
        ProfilePicture = profilePicture;
    }

    public void UpdateDescription(String desc)
    {
        Description = desc;
    }
}