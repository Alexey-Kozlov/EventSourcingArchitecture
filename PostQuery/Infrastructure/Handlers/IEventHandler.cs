using PostCommon.Events;

namespace PostQuery.Infrastructure.Handlers;

public interface IEventHandler
{
    Task On(PostCreatedEvent _event);
    Task On(MessageUpdatedEvent _event);
    Task On(PostLikedEvent _event);
    Task On(CommentAddedEvent _event);
    Task On(CommentUpdatedEvent _event);
    Task On(CommentDeletedEvent _event);
    Task On(PostDeletedEvent _event);
}