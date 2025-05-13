using FriendTagBackend.src.Exceptions;

namespace FriendTagBackend.src.Models.User;
public record Phone
{
    public Phone(string value){
        Value = value;
    }

    public string Value {get; }
}
