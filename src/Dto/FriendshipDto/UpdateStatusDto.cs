using FriendTagBackend.src.Models.Friendship;

namespace FriendTagBackend.src.Dto.FriendshipDto;

public record UpdateStatusDto
{    public Friendship.FStatus Status{ get; set; }
}