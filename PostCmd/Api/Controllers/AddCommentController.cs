using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddCommentController : ControllerBase
{
    private readonly ILogger<AddCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public AddCommentController(ILogger<AddCommentController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> AddCommentAsync(Guid id, AddCommentCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponseDTO
            {
                Message = "Add comment request completed successfully!"
            });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Ошибка добавления комментария");
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseDTO
            {
                Message = $"Ошибка добавления комментария- {ex.Message}"
            });
        }
    }
}
