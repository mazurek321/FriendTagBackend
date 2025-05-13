namespace FriendTagBackend.src.Controllers.Dto.UserDto;

public record UserDto
{
    public string Id {get; set;}
    public string Email {get; set; }
    public string Name {get; set; }
    public string LastName {get; set; }
    public string Phone {get; set; }
    public string Birthday {get; set;}
    public string Latitude {get; set;}
    public string Longitude {get; set;}
    public string? ProfilePicture {get; set;}
    public string? Description {get; set;}
    public string CreatedAt {get; set;}


}