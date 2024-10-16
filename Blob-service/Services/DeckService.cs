using Blob_service.Data;
using Blob_service.Hubs;
using Blob_service.Models;
using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Blob_service.Services
{
    public class DeckService : IDeckService
    {
        private readonly DeckDbContext _db;
        private readonly IHubContext<GameHub> _hub;
        private readonly IScoresService _scoresService;
        private readonly IGameDetailsService _gameDetailsService;

        public DeckService(DeckDbContext db, IHubContext<GameHub> hub, IScoresService scoreService, IGameDetailsService gameDetailsService)
        {
            _db = db;
            _hub = hub;
            _scoresService = scoreService;
            _gameDetailsService = gameDetailsService;
        }

        public string?[] GetHand(string gameID, int player)
        {
            var hand = ConvertToArray(_db.Hands.Where(x => x.GameID == gameID)).Select(x => x[player]).Where(x => x != null).ToArray();
            return hand;
        }

        public ActiveHand? GetActiveHand(string gameID)
        {
            var active = _db.ActiveHand.Where(x => x.GameID == gameID).OrderByDescending(x => x.ID).FirstOrDefault();
            return active;
        }

        public ActiveHand[] GetActiveHands(string gameID)
        {
            var active = _db.ActiveHand.Where(x => x.GameID == gameID).ToArray();
            return active;
        }

        public int FindPreviousWinner(string gameID)
        {
            var gameDetails = _gameDetailsService.GetDetails(gameID);
            var currentScore = _scoresService.GetCurrentScore(gameID);
            var active = GetActiveHand(gameID);
            string?[] played = { active?.PlayerOneCard, active?.PlayerTwoCard, active?.PlayerThreeCard, active?.PlayerFourCard, active?.PlayerFiveCard, active?.PlayerSixCard };
            var previousWinner = ScoresService.FindWinningPlayer(played.Take(gameDetails.NumberOfPlayers).ToArray(), active.LeadingSuit, currentScore.TrumpSuit);

            return previousWinner;
        }

        public int FindPlayerToStartNextRound(string gameID)
        {
            var gameDetails = _gameDetailsService.GetDetails(gameID);
            var currentScore = _scoresService.GetCurrentScore(gameID);
            var player = (currentScore.Round - 1 + gameDetails.NumberOfPlayers) % gameDetails.NumberOfPlayers;

            return player;
        }

        public void PlayCard(string gameID, int player, bool leadingCard, string card)
        {
            var hand = GetHand(gameID, player);
            var numberOfPlayers = _db.GameDetails.Where(x => x.GameID == gameID).First().NumberOfPlayers;
            var nextPlayer = (player + 1) % numberOfPlayers;

            UpdateHand(player, card, _db.Hands.Where(x => x.GameID == gameID));

            if (leadingCard)
            {
                string?[] first = { null, null, null, null, null, null };
                first[player] = card;
                var activeHandRow = new ActiveHand { GameID = gameID, LeadingSuit = card[1], PlayerOneCard = first[0], PlayerTwoCard = first[1], PlayerThreeCard = first[2], PlayerFourCard = first[3], PlayerFiveCard = first[4], PlayerSixCard = first[5] };
                _db.ActiveHand.Add(activeHandRow);
            }
            else
            {
                var active = GetActiveHand(gameID);
                string?[] played = { active.PlayerOneCard, active.PlayerTwoCard, active.PlayerThreeCard, active.PlayerFourCard, active.PlayerFiveCard, active.PlayerSixCard };
                played[player] = card;
                active.PlayerOneCard = played[0];
                active.PlayerTwoCard = played[1];
                active.PlayerThreeCard = played[2];
                active.PlayerFourCard = played[3];
                active.PlayerFiveCard = played[4];
                active.PlayerSixCard = played[5];

                if (!played.Take(numberOfPlayers).Contains(null))
                {
                    nextPlayer = _scoresService.TrickEnded(gameID, played, active.LeadingSuit);
                }
            }

            _db.SaveChanges();

            _hub.Clients.Group(gameID).SendAsync("CardsUpdate", nextPlayer, player, card, leadingCard);
        }

        private static void UpdateHand(int player, string card, IQueryable<SixPlayerHand> table)
        {
            switch (player)
            {
                case 0:
                    var firstRow = table.Single(x => x.PlayerOneCard == card);
                    firstRow.PlayerOneCard = null;
                    break;
                case 1:
                    var secondRow = table.Single(x => x.PlayerTwoCard == card);
                    secondRow.PlayerTwoCard = null;
                    break;
                case 2:
                    var thirdRow = table.Single(x => x.PlayerThreeCard == card);
                    thirdRow.PlayerThreeCard = null;
                    break;
                case 3:
                    var fourthRow = table.Single(x => x.PlayerFourCard == card);
                    fourthRow.PlayerFourCard = null;
                    break;
                case 4:
                    var fifthRow = table.Single(x => x.PlayerFiveCard == card);
                    fifthRow.PlayerFiveCard = null;
                    break;
                case 5:
                    var sixthRow = table.Single(x => x.PlayerSixCard == card);
                    sixthRow.PlayerSixCard = null;
                    break;
            }
        }

        private static string?[][] ConvertToArray(IEnumerable<SixPlayerHand> source)
        {
            var result = new List<string?[]>();

            foreach (var item in source)
            {
                string?[] row = { item.PlayerOneCard, item.PlayerTwoCard, item.PlayerThreeCard, item.PlayerFourCard, item.PlayerFiveCard, item.PlayerSixCard };
                result.Add(row);
            }

            return result.ToArray();
        }
    }
}
