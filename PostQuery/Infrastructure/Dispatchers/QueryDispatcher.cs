using CQRSCore.Infrastructure;
using CQRSCore.Queries;
using PostQuery.Domain.Entities;

namespace PostQuery.Infrastructure.Dispathcers;

public class QueryDispathcer : IQueryDispatcher<Post>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<Post>>>> _handlers = new();
    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<Post>>> handler) where TQuery : BaseQuery
    {
        if (_handlers.ContainsKey(typeof(TQuery)))
        {
            throw new IndexOutOfRangeException($"Нельзя дважды зарегестрировать query handler -{nameof(TQuery)}");
        }
        _handlers.Add(typeof(TQuery), (p) => handler((TQuery)p));
    }

    public async Task<List<Post>> SendAsync(BaseQuery query)
    {
        if (_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<Post>>> handler))
        {
            return await handler(query);
        }
        throw new ArgumentNullException($"{nameof(handler)} - этот хендлер не зарегистрирован");
    }
}