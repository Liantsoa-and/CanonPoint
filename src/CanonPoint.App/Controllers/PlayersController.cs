using CanonPoint.App.Models.Dtos;
using CanonPoint.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanonPoint.App.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly IGameService _gameService;

    public PlayersController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<ActionResult<PlayerResponseDto>> CreatePlayer([FromBody] CreatePlayerRequestDto request, CancellationToken cancellationToken)
    {
        var player = await _gameService.CreatePlayerAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetPlayers), new { id = player.Id }, player);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlayerResponseDto>>> GetPlayers(CancellationToken cancellationToken)
    {
        var players = await _gameService.GetPlayersAsync(cancellationToken);
        return Ok(players);
    }
}
