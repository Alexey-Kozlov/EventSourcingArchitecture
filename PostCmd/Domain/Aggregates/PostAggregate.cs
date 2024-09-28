using CQRSCore.Domain;
using PostCommon.Events;

namespace PostCmd.Domain.Aggregates;

public class PostAggregates : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();
    public bool Active
    {
        get => _active; set => _active = value;
    }
    public PostAggregates() { }
    public PostAggregates(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now
        });
    }
    public void Apply(PostCreatedEvent _event)
    {
        _id = _event.Id;
        _active = true;
        _author = _event.Author;
    }
    public void EditMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidDataException($"Необходимо указать значение сообщения {nameof(message)}");
        }
        if (!_active)
        {
            throw new InvalidOperationException($"Нельзя изменить неактивное сообщение {message}");
        }
        RaiseEvent(new MessageUpdatedEvent
        {
            Id = _id,
            Message = message
        });
    }
    public void Apply(MessageUpdatedEvent _event)
    {
        _id = _event.Id;
    }
    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException($"Нельзя отметить неактивное сообщение");
        }
        RaiseEvent(new PostLikedEvent
        {
            Id = _id
        });
    }
    public void Apply(PostLikedEvent _event)
    {
        _id = _event.Id;
    }
    public void AddComment(string comment, string username)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidDataException($"Необходимо указать значение комментария {nameof(comment)}");
        }
        if (!_active)
        {
            throw new InvalidOperationException($"Нельзя добавить комментарий в неактивное сообщение");
        }
        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            CommentDate = DateTime.Now,
            UserName = username
        });
    }
    public void Apply(CommentAddedEvent _event)
    {
        _id = _event.Id;
        _comments.Add(_event.CommentId, new Tuple<string, string>(_event.Comment, _event.UserName));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidDataException($"Необходимо указать значение комментария {nameof(comment)}");
        }
        if (!_active)
        {
            throw new InvalidOperationException($"Нельзя редактировать комментарий неактивного сообщения");
        }
        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("Ошибка - комментарий может редактировать только автор комментария.");
        }
        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            UserName = username,
            EditDate = DateTime.Now
        });
    }
    public void Apply(CommentUpdatedEvent _event)
    {
        _id = _event.Id;
        _comments[_event.CommentId] = new Tuple<string, string>(_event.Comment, _event.UserName);
    }

    public void DeleteComment(Guid commentid, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException($"Нельзя удалить комментарий неактивного сообщения");
        }
        if (!_comments[commentid].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("Ошибка - комментарий может удалить только автор комментария.");
        }
        RaiseEvent(new CommentDeletedEvent
        {
            Id = _id,
            CommentId = commentid
        });
    }

    public void Apply(CommentDeletedEvent _event)
    {
        _id = _event.Id;
        _comments.Remove(_event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException($"Пост уже был удален");
        }
        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("Ошибка - пост может удалить только автор поста.");
        }
        RaiseEvent(new PostDeletedEvent
        {
            Id = _id
        });
    }
    public void Apply(PostDeletedEvent _event)
    {
        _id = _event.Id;
        _active = false;
    }
}