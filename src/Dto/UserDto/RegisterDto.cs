using System.ComponentModel.DataAnnotations;
namespace FriendTagBackend.src.Controllers.Dto.UserDto;

public record RegisterDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email {get; set;}

    [Required(ErrorMessage = "Name is required.")]
    public string Name {get; set;}

    [Required(ErrorMessage = "Lastname is required.")]
    public string Lastname {get; set;}

    [Required(ErrorMessage = "Password is required.")]
    [Compare("ConfirmPassword", ErrorMessage ="Password does not match.")]
    public string Password {get; set;}

    [Required(ErrorMessage = "Confirm password is required.")]
    public string ConfirmPassword {get; set;}

    [Required(ErrorMessage = "Phone number is required.")]
    public string Phone {get; set;}

    public string Latitude {get; set;}
    public string Longitude {get; set;}
    public string? ProfilePicture {get; set;}
    public string? Description {get; set;}
}