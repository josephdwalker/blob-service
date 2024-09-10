using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blob_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly IBidsService _bidsService;
        private readonly IGameDetailsService _gameDetailsService;

        public BidsController(IBidsService bidsService, IGameDetailsService gameDetailsService)
        {
            _bidsService = bidsService;
            _gameDetailsService = gameDetailsService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{gameID}/gameID/getBids")]
        public ActionResult GetBids(int gameID)
        {
            var bids = _bidsService.GetBids(gameID);
            return Ok(bids);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/{player}/player/{bid}/bid/setBid")]
        public ActionResult SetBid(int gameID, int player, int bid)
        {
            if (bid % 1 != 0 || bid < 0)
            {
                return BadRequest("Bid must be a non-negative integer");
            };

            var gameDetails = _gameDetailsService.GetDetails(gameID);

            if (!new List<int> { 0, 1, 2, 3, 4, 5 }.Take(gameDetails.NumberOfPlayers).Contains(player))
            {
                return BadRequest("Not a valid player");
            }

            var bidList = _bidsService.GetBids(gameID);

            if (bidList[player] != null)
            {
                return BadRequest("You have already bidded");
            }

            if (bidList.Where(x => x == null).Count() == 1)
            {
                if (bidList.Sum() + bid == _bidsService.GetTricks(gameID))
                {
                    return BadRequest("You are forced, invalid bid");
                }
            }

            bidList[player] = bid;
            _bidsService.SetBids(gameID, player, bidList);

            return Ok();
        }
    }
}
