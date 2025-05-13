using FriendTagBackend.src.Exceptions;

namespace FriendTagBackend.src.Models.User;
public record Name
{
    public Name(string value){
        if(string.IsNullOrWhiteSpace(value)) throw new CustomException("Invalid name.");
        Value = value;
    }
    
    public string Value {get; }
}