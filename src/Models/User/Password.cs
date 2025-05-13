using System.Security.Cryptography;
using System.Text;
using FriendTagBackend.src.Exceptions;

namespace FriendTagBackend.src.Models.User;
public record Password
{
    public Password(string value){
        if(string.IsNullOrWhiteSpace(value)) throw new CustomException("Invalid id.");
        Value = value;
    }
    
    public string Value {get; }

    public string CalculateMD5Hash(string input)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for(int i=0; i< hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }
}