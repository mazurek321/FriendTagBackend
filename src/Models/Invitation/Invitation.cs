using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Models.Invitation;

public class Invitation
{ 
    private Invitation(Guid id, Guid eventId, UserId invitedPerson, InvitationStatus status)
    {
        Id = id;
        EventId = eventId;  
        InvitedPerson = invitedPerson;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid EventId {get; private set; }
    public UserId InvitedPerson {get; private set; }
    public InvitationStatus Status{get; private set; }

    public enum InvitationStatus{
        Sent,
        Accepted,
        Declined,
        Request
    }

    public static Invitation SendInvitation(Guid eventId, UserId invitedPerson)
    {
        return new Invitation(Guid.NewGuid(), eventId, invitedPerson, InvitationStatus.Sent);
    }

    public static Invitation RequestInvitation(Guid eventId, UserId requester)
    {
        return new Invitation(Guid.NewGuid(), eventId, requester, InvitationStatus.Request);
    }

    public void UpdateInvitation(InvitationStatus status)
    {
        Status = status;
    }
}