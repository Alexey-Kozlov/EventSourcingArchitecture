using CQRSCore.Commands;

namespace PostCmd.Api.Commands;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string UserName { get; set; }
}