using CQRSCore.Events;

namespace PostCommon.Events;

public class PostDeletedEvent : BaseEvent
{
    public PostDeletedEvent() : base(nameof(PostDeletedEvent))
    {

    }
}