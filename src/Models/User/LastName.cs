using FriendTagBackend.src.Exceptions;

namespace FriendTagBackend.src.Models.User;
public record LastName
{
    public LastName(string value){
        if(string.IsNullOrWhiteSpace(value) || value.Length is > 200 or < 3) throw new CustomException("Invalid lastname.");
        Value = value;
    }
    
    public string Value {get; }
}