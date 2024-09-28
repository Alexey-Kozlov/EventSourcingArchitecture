using CQRSCore.Commands;

namespace PostCmd.Api.Commands;

public class DeleteCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string UserName { get; set; }
}