using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class EditCommentController : ControllerBase
{
    private readonly ILogger<EditCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public EditCommentController(ILogger<EditCommentController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditCommentAsync(Guid id, EditCommentCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponseDTO
            {
                Message = "Комментарий отредактирован"
            });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Ошибка редактирования комментария");
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseDTO
            {
                Message = $"Ошибка редактирования комментария - {ex.Message}"
            });
        }
    }
}
