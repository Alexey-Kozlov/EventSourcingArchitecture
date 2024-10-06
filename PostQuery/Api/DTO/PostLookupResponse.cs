using PostCommon.DTO;
using PostQuery.Domain.Entities;

namespace PostQuery.Api.DTO;

public class PostLookupResponse : BaseResponseDTO
{
    public List<Post> Posts { get; set; }
}