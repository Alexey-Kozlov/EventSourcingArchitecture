using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikePostController : ControllerBase
{
    private readonly ILogger<LikePostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public LikePostController(ILogger<LikePostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }
    [HttpPost("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id)
    {
        try
        {
            await _commandDispatcher.SendAsync(new LikePostCommand { Id = id });
            return Ok(new BaseResponseDTO
            {
                Message = "Запрос like сообщения завершен"
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