using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EditMessageController : ControllerBase
{
    private readonly ILogger<EditMessageController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }
    [HttpPost("{id}")]
    public async Task<ActionResult> EditMessageAsync(Guid id, EditMessageCommand editMessageCommand)
    {
        try
        {
            editMessageCommand.Id = id;
            await _commandDispatcher.SendAsync(editMessageCommand);
            return Ok(new BaseResponseDTO
            {
                Message = "Запрос на редактирование сообщения завершен"
            });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Ошибка редактирования сообщения");
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseDTO
            {
                Message = $"Ошибка редактирования сообщения - {ex.Message}"
            });
        }
    }
}