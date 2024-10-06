using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DeleteCommentController : ControllerBase
{
    private readonly ILogger<DeleteCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public DeleteCommentController(ILogger<DeleteCommentController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveCommentAsync(Guid id, DeleteCommentCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponseDTO
            {
                Message = "Комментарий успешно удален"
            });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Ошибка удаления комментария");
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseDTO
            {
                Message = $"Ошибка удаления комментария - {ex.Message}"
            });
        }
    }
}
