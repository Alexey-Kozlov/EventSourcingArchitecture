using PostQuery.Domain.Entities;
using PostQuery.Domain.Repositories;

namespace PostQuery.Api.Queries;

public class QueryHandler : IQueryHandler
{
    private readonly IPostRepository _postRepository;

    public QueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> HandleAsync(FindAllPostsQuery query)
    {
        return await _postRepository.GetAllAsync();
    }

    public async Task<List<Post>> HandleAsync(FindPostByIdQuery query)
    {
        var post = await _postRepository.GetByIdAsync(query.Id);
        return new List<Post> { post };
    }

    public async Task<List<Post>> HandleAsync(FindPostsByAuthorQuery query)
    {
        return await _postRepository.GetByAuthorAsync(query.Author);
    }

    public async Task<List<Post>> HandleAsync(FindPostsWithCommentQuery query)
    {
        return await _postRepository.GetWithCommentsAsync();
    }

    public async Task<List<Post>> HandleAsync(FindPostsWithLikeQuery query)
    {
        return await _postRepository.GetWithLikeAsync(query.NumberOfLikes);
    }
}