using Blob_service.Models;
using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blob_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IScoresService _scoreService;

        public ScoreController(IScoresService scoreService)
        {
            _scoreService = scoreService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("startGame")]
        public IActionResult StartGame([FromBody] GameDetails gameDetails)
        {
            _scoreService.StartGame(gameDetails);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{gameID}/gameID/getScores")]
        public IActionResult GetScores(string gameID)
        {
            var scores = _scoreService.GetScores(gameID);
            return Ok(scores);
        }
    }
}
