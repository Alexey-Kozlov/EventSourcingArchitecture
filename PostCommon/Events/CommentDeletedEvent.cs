using CQRSCore.Events;

namespace PostCommon.Events;

public class CommentDeletedEvent : BaseEvent
{
    public Guid CommentId { get; set; }
    public CommentDeletedEvent() : base(nameof(CommentDeletedEvent))
    {

    }
}