using CQRSCore.Events;

namespace PostCommon.Events;

public class CommentUpdatedEvent : BaseEvent
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string UserName { get; set; }
    public DateTime EditDate { get; set; }
    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
    {

    }
}