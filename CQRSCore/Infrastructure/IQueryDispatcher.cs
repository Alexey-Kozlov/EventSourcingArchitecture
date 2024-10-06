using CQRSCore.Queries;

namespace CQRSCore.Infrastructure;

public interface IQueryDispatcher<T>
{
    void RegisterHandler<TQuery>(Func<TQuery, Task<List<T>>> handler) where TQuery : BaseQuery;
    Task<List<T>> SendAsync(BaseQuery query);
}