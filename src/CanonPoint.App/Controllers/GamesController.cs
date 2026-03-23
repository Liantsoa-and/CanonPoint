using CanonPoint.App.Models.Dtos;
using CanonPoint.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace CanonPoint.App.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<ActionResult<GameResponseDto>> CreateGame([FromBody] CreateGameRequestDto request, CancellationToken cancellationToken)
    {
        if (request.GridRows <= 0 || request.GridCols <= 0)
        {
            return BadRequest("GridRows et GridCols doivent etre superieurs a 0.");
        }

        var result = await _gameService.CreateGameAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetGameState), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameResponseDto>>> GetGames(CancellationToken cancellationToken)
    {
        var games = await _gameService.GetGamesAsync(cancellationToken);
        return Ok(games);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameStateResponseDto>> GetGameState(int id, CancellationToken cancellationToken)
    {
        var game = await _gameService.GetGameStateAsync(id, cancellationToken);
        if (game is null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    [HttpPost("{id:int}/points")]
    public async Task<ActionResult<MoveResponseDto>> AddPoint(int id, [FromBody] AddPointRequestDto request, CancellationToken cancellationToken)
    {
        var move = await _gameService.AddPointAsync(id, request, cancellationToken);
        if (move is null)
        {
            return NotFound("Partie ou joueur introuvable.");
        }

        return Ok(move);
    }

    [HttpPost("{id:int}/shots")]
    public async Task<ActionResult<MoveResponseDto>> FireShot(int id, [FromBody] FireShotRequestDto request, CancellationToken cancellationToken)
    {
        if (request.Power < 1 || request.Power > 9)
        {
            return BadRequest("Power doit etre compris entre 1 et 9.");
        }

        var move = await _gameService.FireShotAsync(id, request, cancellationToken);
        if (move is null)
        {
            return NotFound("Partie ou joueur introuvable.");
        }

        return Ok(move);
    }
}
