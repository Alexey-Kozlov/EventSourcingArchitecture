
using CQRSCore.Handlers;
using PostCmd.Domain.Aggregates;

namespace PostCmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourceHandler<PostAggregates> _eventSourceHandler;

    public CommandHandler(IEventSourceHandler<PostAggregates> eventSourceHandler)
    {
        _eventSourceHandler = eventSourceHandler;
    }

    public async Task HandlerAsync(NewPostCommand command)
    {
        var aggregate = new PostAggregates(command.Id, command.Author, command.Message);
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(EditMessageCommand command)
    {
        var aggregate = await _eventSourceHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message);
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(LikePostCommand command)
    {
        var aggregate = await _eventSourceHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(AddCommentCommand command)
    {
        var aggregate = await _eventSourceHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.UserName);
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(EditCommentCommand command)
    {
        var aggregate = await _eventSourceHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.UserName);
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(DeleteCommentCommand command)
    {
        var aggregate = await _eventSourceHandler.GetByIdAsync(command.Id);
        aggregate.DeleteComment(command.CommentId, command.UserName);
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(DeletePostCommand command)
    {
        var aggregate = await _eventSourceHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.UserName);
        await _eventSourceHandler.SaveAsync(aggregate);
    }

    public async Task HandlerAsync(RestoreReadDbCommand command)
    {
        await _eventSourceHandler.RepublishEventsAsync();
    }
}