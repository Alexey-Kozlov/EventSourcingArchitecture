using CQRSCore.Queries;

namespace PostQuery.Api.Queries;

public class FindPostsWithLikeQuery : BaseQuery
{
    public int NumberOfLikes { get; set; }
}