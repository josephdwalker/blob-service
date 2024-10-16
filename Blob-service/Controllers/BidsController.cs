﻿using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blob_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly IBidsService _bidsService;
        private readonly IScoresService _scoresService;
        private readonly IDeckService _deckService;
        private readonly IGameDetailsService _gameDetailsService;

        public BidsController(IBidsService bidsService, IScoresService scoresService, IDeckService deckService, IGameDetailsService gameDetailsService)
        {
            _bidsService = bidsService;
            _scoresService = scoresService;
            _deckService = deckService;
            _gameDetailsService = gameDetailsService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{gameID}/gameID/getBids")]
        public ActionResult GetBids(string gameID)
        {
            var bids = _bidsService.GetBids(gameID);
            return Ok(bids);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/{player}/player/{bid}/bid/setBid")]
        public ActionResult SetBid(string gameID, int player, int bid)
        {
            if (bid % 1 != 0 || bid < 0)
            {
                return BadRequest("Bid must be a non-negative integer");
            };

            var gameDetails = _gameDetailsService.GetDetails(gameID);

            if (gameDetails == null)
            {
                return BadRequest("Not a valid game");
            }

            if (!new List<int> { 0, 1, 2, 3, 4, 5 }.Take(gameDetails.NumberOfPlayers).Contains(player))
            {
                return BadRequest("Not a valid player");
            }

            var bidList = _bidsService.GetBids(gameID);

            if (bidList[player] != null)
            {
                return BadRequest("You have already bidded");
            }

            var nonNullBids = bidList.Where(x => x != null);

            if (!nonNullBids.Any())
            {
                var playerToStartNextRound = _deckService.FindPlayerToStartNextRound(gameID);

                if (playerToStartNextRound != player)
                {
                    return BadRequest("Not your turn to bid");
                }
            }
            else
            {
                var previousPlayer = (player - 1 + gameDetails.NumberOfPlayers) % gameDetails.NumberOfPlayers;

                if (bidList[previousPlayer] == null)
                {
                    return BadRequest("Not your turn to bid");
                }
            }

            if (bidList.Where(x => x == null).Count() == 1)
            {
                if (bidList.Sum() + bid == _scoresService.GetCurrentScore(gameID).Tricks)
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
