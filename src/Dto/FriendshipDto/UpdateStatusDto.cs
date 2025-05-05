using FriendTagBackend.src.Models.Friendship;

namespace FriendTagBackend.src.Dto.FriendshipDto;

public record UpdateStatusDto
{
    public Guid FriendId { get; set; }
    public Friendship.FStatus Status{ get; set; }
}