using CQRSCore.Queries;

namespace PostQuery.Api.Queries;

public class FindPostsByAuthorQuery : BaseQuery
{
    public string Author { get; set; }
}