using PostQuery.Domain.Entities;

namespace PostQuery.Domain.Repositories;

public interface ICommentRepository
{
    Task CreateAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task<Comment> GetByIdAsync(Guid commentId);
    Task DeleteAsync(Guid commentId);
}