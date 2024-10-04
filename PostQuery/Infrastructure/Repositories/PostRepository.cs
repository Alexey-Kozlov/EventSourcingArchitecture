using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostQuery.Domain.Entities;
using PostQuery.Domain.Repositories;
using PostQuery.Infrastructure.DBContext;

namespace PostQuery.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DataContextFactory _dataContextFactory;
    private readonly ILogger<PostRepository> _logger;

    public PostRepository(DataContextFactory dataContextFactory, ILogger<PostRepository> logger)
    {
        _dataContextFactory = dataContextFactory;
        _logger = logger;
    }

    public async Task CreateAsync(Post post)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        context.Posts.Add(post);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Ошибка создания Post - {e.Message}");
            if (e.InnerException != null)
            {
                _logger.LogError($"Ошибка создания Post - {e.InnerException.Message}");
            }
        }

    }

    public async Task DeleteAsync(Guid postId)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);
        if (post == null) return;
        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<List<Post>> GetAllAsync()
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(x => x.Comments).AsNoTracking().ToListAsync();
    }

    public async Task<List<Post>> GetByAuthorAsync(string author)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(x => x.Comments).AsNoTracking().Where(x => x.Author == author).ToListAsync();
    }

    public async Task<Post> GetByIdAsync(Guid id)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(x => x.Comments).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Post>> GetWithCommentsAsync()
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(x => x.Comments).AsNoTracking()
            .Where(x => x.Comments != null && x.Comments.Any()).ToListAsync();
    }

    public async Task<List<Post>> GetWithLikeAsync(int numberOfLike)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(x => x.Comments).AsNoTracking()
            .Where(x => x.Likes >= numberOfLike).ToListAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }
}