using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Models.Blocked;

public class Blocked{
    public Blocked(){}
    private Blocked(Guid id, UserId blocker, UserId blockedPerson)
    {
        Id = id;
        Blocker = blocker;
        BlockedPerson = blockedPerson;
    }
    public Guid Id { get; private set; }
    public UserId Blocker{ get; private set; }
    public UserId BlockedPerson{ get; private set; }

    public static Blocked Block(UserId blocker, UserId blockedPerson)
    {
        return new Blocked(Guid.NewGuid(), blocker, blockedPerson);
    }
}
