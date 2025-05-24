using FriendTagBackend.src.Models.User;

namespace FriendTagBackend.src.Models.Message;

public class Message
{ 
    public Message(){}
    private Message(Guid id, UserId senderId, UserId receiverId, String content,
                    DateTime sentAt, String? imageUrl,
                    User.User sender, User.User receiver
    )
    {
        Id = id;    
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        SentAt = sentAt;
        ImageUrl = imageUrl;
        Sender = sender;
        Receiver = receiver;
    }
    public Guid Id { get; private set; }
    public UserId SenderId { get; private set; }
    public UserId ReceiverId { get; private set; }
    public String Content { get; private set; }
    public DateTime SentAt {get; private set; }
    public String? ImageUrl { get; private set; }
    public User.User Sender { get; private set; }
    public User.User Receiver { get; private set; }

    public static Message NewMessage(UserId senderId, UserId receiverId, String content,
                    DateTime sentAt, String? imageUrl,
                    User.User sender, User.User receiver)
    {
        return new Message(Guid.NewGuid(), senderId, receiverId, content, sentAt, imageUrl,
                                            sender, receiver); 
    }
}