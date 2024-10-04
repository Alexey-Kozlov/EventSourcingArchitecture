using PostQuery.Domain.Entities;

namespace PostQuery.Domain.Repositories;

public interface IPostRepository
{
    Task CreateAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Guid postId);
    Task<Post> GetByIdAsync(Guid id);
    Task<List<Post>> GetAllAsync();
    Task<List<Post>> GetByAuthorAsync(string author);
    Task<List<Post>> GetWithLikeAsync(int numberOfLike);
    Task<List<Post>> GetWithCommentsAsync();
}