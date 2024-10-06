using CQRSCore.Queries;

namespace PostQuery.Api.Queries;

public class FindPostByIdQuery : BaseQuery
{
    public Guid Id { get; set; }
}