using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Models.Friendship;

public class Friendship
{     
    public Friendship(){}

    private Friendship(Guid id, UserId user1, UserId user2,UserId requestSender, FStatus status)
    {
        Id = id;
        User1 = user1;
        User2 = user2;
        RequestSender = requestSender;
        Status = status;
    }

    public Guid Id { get; private set; }
    public UserId User1 { get; private set; }
    public UserId User2 { get; private set; }
    public UserId RequestSender { get; private set; }
    public FStatus Status { get; private set; }


    public static Friendship AddFriend(UserId u1, UserId u2, UserId rs)
    {
        return new Friendship(Guid.NewGuid(), u1, u2, rs, FStatus.Pending);
    }

    public void SetStatus(FStatus status)
    {
        Status = status;
    }

    public enum FStatus{
        Pending,
        Accepted,
        Rejected
    }
}