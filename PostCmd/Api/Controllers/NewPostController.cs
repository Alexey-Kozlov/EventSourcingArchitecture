using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewPostController : ControllerBase
{
    private readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand newPostCommand)
    {
        newPostCommand.Id = Guid.NewGuid();
        try
        {
            await _commandDispatcher.SendAsync(newPostCommand);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponseDTO
            {
                Id = newPostCommand.Id,
                Message = "Запрос на создание нового поста завершен"
            });
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, "Ошибка создания нового поста");
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponseDTO
            {
                Id = newPostCommand.Id,
                Message = $"Ошибка содания нового поста - {ex.Message}"
            });
        }
    }
}