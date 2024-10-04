using PostCommon.Events;
using PostQuery.Domain.Entities;
using PostQuery.Domain.Repositories;

namespace PostQuery.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task On(PostCreatedEvent _event)
    {
        var post = new Post
        {
            Id = _event.Id,
            Author = _event.Author,
            Message = _event.Message,
            DatePosted = _event.DatePosted.ToUniversalTime()
        };
        await _postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdatedEvent _event)
    {
        var post = await _postRepository.GetByIdAsync(_event.Id);
        if (post == null) return;
        post.Message = _event.Message;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(PostLikedEvent _event)
    {
        var post = await _postRepository.GetByIdAsync(_event.Id);
        if (post == null) return;
        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent _event)
    {
        var comment = new Comment
        {
            Id = _event.CommentId,
            PostId = _event.Id,
            CommentDate = _event.CommentDate.ToUniversalTime(),
            CommentText = _event.Comment,
            UserName = _event.UserName,
            Edited = false
        };
        await _commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent _event)
    {
        var comment = await _commentRepository.GetByIdAsync(_event.CommentId);
        if (comment == null) return;
        comment.CommentText = _event.Comment;
        comment.Edited = true;
        comment.CommentDate = _event.EditDate.ToUniversalTime();
        await _commentRepository.UpdateAsync(comment);
    }

    public async Task On(CommentDeletedEvent _event)
    {
        await _commentRepository.DeleteAsync(_event.CommentId);
    }

    public async Task On(PostDeletedEvent _event)
    {
        await _postRepository.DeleteAsync(_event.Id);
    }
}