using FriendTagBackend.src.Exceptions;

namespace FriendTagBackend.src.Models.User;
public record Phone
{
    public Phone(string value){
        if(value.Length is > 15 or < 8) throw new CustomException("Invalid phone number.");
        Value = value;
    }

    public string Value {get; }
}
