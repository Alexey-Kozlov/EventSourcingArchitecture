using CQRSCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PostCmd.Api.Commands;
using PostCommon.DTO;

namespace PostCmd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestoreReadDbController : ControllerBase
{
    private readonly ILogger<RestoreReadDbController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> RestoreReadDbAsync()
    {
        try
        {
            await _commandDispatcher.SendAsync(new RestoreReadDbCommand());

            return StatusCode(StatusCodes.Status201Created, new BaseResponseDTO
            {
                Message = "Read database restore request completed successfully!"
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to restore read database!";
            _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseDTO
            {
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}
