using CQRSCore.Commands;

namespace PostCmd.Api.Commands;

public class DeletePostCommand : BaseCommand
{
    public string UserName { get; set; }
}
