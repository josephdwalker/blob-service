using Blob_service.Services;
using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blob_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeckController : ControllerBase
    {
        private readonly IDeckService _deckService;
        private readonly IBidsService _bidsService;
        private readonly IScoresService _scoresService;
        private readonly IGameDetailsService _gameDetailsService;

        public DeckController(IDeckService deckService, IBidsService bidsService, IScoresService scoresService, IGameDetailsService gameDetailsService)
        {
            _deckService = deckService;
            _bidsService = bidsService;
            _scoresService = scoresService;
            _gameDetailsService = gameDetailsService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/{player}/player/getHand")]
        public ActionResult GetHand(string gameID, int player)
        {
            var gameDetails = _gameDetailsService.GetDetails(gameID);

            if (gameDetails == null)
            {
                return BadRequest("Not a valid game");
            }

            if (!new List<int>{ 0, 1, 2, 3, 4, 5 }.Take(gameDetails.NumberOfPlayers).Contains(player))
            {
                return BadRequest("Not a valid player");
            }

            var hand = _deckService.GetHand(gameID, player);

            return Ok(hand);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/getActiveHands")]
        public ActionResult GetActiveHands(string gameID)
        {
            var hands = _deckService.GetActiveHands(gameID);
            return Ok(hands);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/{player}/player/{card}/card/playCard")]
        public ActionResult PlayCard(string gameID, int player, string card)
        {
            var gameDetails = _gameDetailsService.GetDetails(gameID);

            if (gameDetails == null)
            {
                return BadRequest("Not a valid game");
            }

            if (!new List<int> { 0, 1, 2, 3, 4, 5 }.Take(gameDetails.NumberOfPlayers).Contains(player))
            {
                return BadRequest("Not a valid player");
            }

            var bids = _bidsService.GetBids(gameID);

            if (bids.Contains(null))
            {
                return BadRequest("Bidding is not complete");
            }

            var hand = _deckService.GetHand(gameID, player);

            if (!hand.Contains(card))
            {
                return BadRequest("That card is not in your hand");
            }

            var active = _deckService.GetActiveHand(gameID);
            string?[] played = { active?.PlayerOneCard, active?.PlayerTwoCard, active?.PlayerThreeCard, active?.PlayerFourCard, active?.PlayerFiveCard, active?.PlayerSixCard };

            var activeHands = _deckService.GetActiveHands(gameID);
            var leadingCard = activeHands.Length == 0 || !played.Take(gameDetails.NumberOfPlayers).Contains(null);

            if (leadingCard)
            {
                var nextPlayerToStartRound = _deckService.FindPlayerToStartNextRound(gameID);
                if (activeHands.Length == 0 && player != nextPlayerToStartRound)
                {
                    return BadRequest("Not your turn to play a card");
                }

                if (!played.Take(gameDetails.NumberOfPlayers).Contains(null))
                {
                    var previousWinner = _deckService.FindPreviousWinner(gameID);

                    if (previousWinner != player)
                    {
                        return BadRequest("Not your turn to play a card");
                    }
                }
            }
            else
            {
                if (played[player] != null)
                {
                    return BadRequest("You have already played a card");
                }

                var previousPlayer = (player - 1 + gameDetails.NumberOfPlayers) % gameDetails.NumberOfPlayers;

                if (played.Take(gameDetails.NumberOfPlayers).ToArray()[previousPlayer] == null)
                {
                    return BadRequest("Not your turn to play a card");
                }

                if (!leadingCard && active?.LeadingSuit != card[1] && hand.Select(x => x?[1]).Contains(active?.LeadingSuit))
                {
                    return BadRequest("You must follow suit");
                }
            }

            _deckService.PlayCard(gameID, player, leadingCard, card);

            return Ok();
        }
    }
}
