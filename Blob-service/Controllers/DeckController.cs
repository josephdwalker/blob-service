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
        private readonly IGameDetailsService _gameDetailsService;

        public DeckController(IDeckService deckService, IBidsService bidsService, IGameDetailsService gameDetailsService)
        {
            _deckService = deckService;
            _bidsService = bidsService;
            _gameDetailsService = gameDetailsService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/{player}/player/getHand")]
        public ActionResult GetHand(int gameID, int player)
        {
            var gameDetails = _gameDetailsService.GetDetails(gameID);

            if (!new List<int>{ 0, 1, 2, 3, 4, 5 }.Take(gameDetails.NumberOfPlayers).Contains(player))
            {
                return BadRequest("Not a valid player");
            }

            var hand = _deckService.GetHand(gameID, player);

            return Ok(hand);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{gameID}/gameID/{player}/player/{leadingCard}/leadingCard/{card}/card/playCard")]
        public ActionResult PlayCard(int gameID, int player, bool leadingCard, string card)
        {
            var gameDetails = _gameDetailsService.GetDetails(gameID);

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

            if (!leadingCard)
            {
                var active = _deckService.GetActiveHand(gameID);
                string?[] played = { active.PlayerOneCard, active.PlayerTwoCard, active.PlayerThreeCard, active.PlayerFourCard, active.PlayerFiveCard, active.PlayerSixCard };

                if (played[player] != null)
                {
                    return BadRequest("You have already played a card");
                }

                if (active.LeadingSuit != card[1])
                {
                    var handSuits = hand.Where(x => x != null).Select(x => x?[1]);
                    if (handSuits.Contains(active.LeadingSuit))
                    {
                        return BadRequest("You must follow suit");
                    }
                }
            }

            _deckService.PlayCard(gameID, player, leadingCard, card);

            return Ok();
        }
    }
}
