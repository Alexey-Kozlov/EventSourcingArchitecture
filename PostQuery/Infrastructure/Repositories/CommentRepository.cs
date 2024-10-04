using Microsoft.EntityFrameworkCore;
using PostQuery.Domain.Entities;
using PostQuery.Domain.Repositories;
using PostQuery.Infrastructure.DBContext;

namespace PostQuery.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DataContextFactory _dataContextFactory;

    public CommentRepository(DataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }
    public async Task CreateAsync(Comment comment)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        var comment = await GetByIdAsync(commentId);
        if (comment == null) return;
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<Comment> GetByIdAsync(Guid commentId)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        return await context.Comments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task UpdateAsync(Comment comment)
    {
        using DataContext context = _dataContextFactory.CreateDbContext();
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }
}