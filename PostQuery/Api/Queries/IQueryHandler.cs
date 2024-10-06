using PostQuery.Domain.Entities;

namespace PostQuery.Api.Queries;

public interface IQueryHandler
{
    Task<List<Post>> HandleAsync(FindAllPostsQuery query);
    Task<List<Post>> HandleAsync(FindPostByIdQuery query);
    Task<List<Post>> HandleAsync(FindPostsByAuthorQuery query);
    Task<List<Post>> HandleAsync(FindPostsWithCommentQuery query);
    Task<List<Post>> HandleAsync(FindPostsWithLikeQuery query);
}