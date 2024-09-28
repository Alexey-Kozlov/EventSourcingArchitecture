using CQRSCore.Commands;

namespace PostCmd.Api.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Message { get; set; }
}