using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class DeletePostController : ControllerBase
{
    private readonly ILogger<DeletePostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public DeletePostController(ILogger<DeletePostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePostAsync(Guid id, DeletePostCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponseDTO
            {
                Message = "Пост успешно удален"
            });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Ошибка удаления поста");
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseDTO
            {
                Message = $"Ошибка удаления поста - {ex.Message}"
            });
        }
    }
}
