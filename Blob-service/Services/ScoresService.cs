using Blob_service.Data;
using Blob_service.Hubs;
using Blob_service.Models;
using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Blob_service.Services
{
    public class ScoresService : IScoresService
    {
        private readonly DeckDbContext _db;
        private readonly IHubContext<GameHub> _hub;
        private readonly char[] Trumps = { 'H', 'C', 'D', 'S' };
        private readonly char[] NoTrumps = { 'H', 'C', 'D', 'S', 'N' };

        public ScoresService(DeckDbContext db, IHubContext<GameHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        public void StartGame(GameDetails gameDetails)
        {
            DealCards(gameDetails.GameID, 1, gameDetails.NumberOfPlayers);

            _db.GameDetails.Add(gameDetails);

            _db.Bids.Add(new Bids { GameID = gameDetails.GameID });

            var totalRounds = gameDetails.NumberOfRounds;

            for (var i = 0; i < totalRounds; i++)
            {
                var trump = gameDetails.NoTrumpsRound ? NoTrumps[i%NoTrumps.Length] : Trumps[i%Trumps.Length];

                var newTricks = i < totalRounds / 2 ? i + 1 : totalRounds - i;

                var score = new Scores { GameID = gameDetails.GameID, Round = i + 1, Tricks = newTricks, TrumpSuit = trump };

                _db.Scores.Add(score);
            }

            _db.SaveChanges();

            var first = _db.Scores.Where(x => x.GameID ==  gameDetails.GameID).OrderBy(x => x.ID).First();

            first.PlayerOneScore = 0;
            first.PlayerTwoScore = 0;
            if (gameDetails.NumberOfPlayers >= 3)
            {
                first.PlayerThreeScore = 0;
            }
            if (gameDetails.NumberOfPlayers >= 4)
            {
                first.PlayerFourScore = 0;
            }
            if (gameDetails.NumberOfPlayers >= 5)
            {
                first.PlayerFiveScore = 0;
            }
            if (gameDetails.NumberOfPlayers == 6)
            {
                first.PlayerSixScore = 0;
            }

            _db.SaveChanges();

            _hub.Clients.Group(gameDetails.GameID).SendAsync("BidUpdate", 0);
        }

        public CumulativeScores[] GetScores(string gameID)
        {
            var scores = _db.Scores.Where(x => x.GameID == gameID).ToArray();
            List<CumulativeScores> cumulativeScores = new();

            for (var i=0; i<scores.Length; i++)
            {
                var score = scores[i];
                var cumulative = scores.Take(i + 1);
                var nonEmptyRound = score.PlayerOneScore > 0 || score.PlayerTwoScore > 0 || score.PlayerThreeScore > 0 || score.PlayerFourScore > 0 || score.PlayerFiveScore > 0 || score.PlayerSixScore > 0;

                if (nonEmptyRound)
                {
                    var newScore = new CumulativeScores
                    {
                        Tricks = score.Tricks,
                        TrumpSuit = score.TrumpSuit,
                        PlayerOneScore = score.PlayerOneScore,
                        PlayerOneCumulativeScore = cumulative.Select(x => x.PlayerOneScore).Sum(),
                        PlayerTwoScore = score.PlayerTwoScore,
                        PlayerTwoCumulativeScore = cumulative.Select(x => x.PlayerTwoScore).Sum(),
                        PlayerThreeScore = score.PlayerThreeScore,
                        PlayerThreeCumulativeScore = cumulative.Select(x => x.PlayerThreeScore).Sum(),
                        PlayerFourScore = score.PlayerFourScore,
                        PlayerFourCumulativeScore = cumulative.Select(x => x.PlayerFourScore).Sum(),
                        PlayerFiveScore = score.PlayerFiveScore,
                        PlayerFiveCumulativeScore = cumulative.Select(x => x.PlayerFiveScore).Sum(),
                        PlayerSixScore = score.PlayerSixScore,
                        PlayerSixCumulativeScore = cumulative.Select(x => x.PlayerSixScore).Sum()
                    };

                    cumulativeScores.Add(newScore);
                }
                else
                {
                    var emptyScore = new CumulativeScores
                    {
                        Tricks = score.Tricks,
                        TrumpSuit = score.TrumpSuit
                    };

                    cumulativeScores.Add(emptyScore);
                }
            }

            return cumulativeScores.ToArray();
        }

        private void DealCards(string gameID, int tricks, int numberOfPlayers)
        {
            char[] values = { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
            var deck = new List<string>();

            foreach (char v in values)
            {
                foreach (char s in Trumps)
                {
                    deck.Add($"{v}{s}");
                }
            }

            Random rnd = new();

            for (var i = 0; i < tricks; i++)
            {
                var playerOneCard = deck[rnd.Next(deck.Count)];
                deck.Remove(playerOneCard);

                var playerTwoCard = deck[rnd.Next(deck.Count)];
                deck.Remove(playerTwoCard);

                var playerThreeCard = numberOfPlayers >= 3 ? deck[rnd.Next(deck.Count)] : null;
                if (playerThreeCard != null)
                {
                    deck.Remove(playerThreeCard);
                }

                var playerFourCard = numberOfPlayers >= 4 ? deck[rnd.Next(deck.Count)] : null;
                if (playerFourCard != null)
                {
                    deck.Remove(playerFourCard);
                }

                var playerFiveCard = numberOfPlayers >= 5 ? deck[rnd.Next(deck.Count)] : null;
                if (playerFiveCard != null)
                {
                    deck.Remove(playerFiveCard);
                }

                var playerSixCard = numberOfPlayers == 6 ? deck[rnd.Next(deck.Count)] : null;
                if (playerSixCard != null)
                {
                    deck.Remove(playerSixCard);
                }

                var hand = new SixPlayerHand { GameID = gameID, PlayerOneCard = playerOneCard, PlayerTwoCard = playerTwoCard, PlayerThreeCard = playerThreeCard, PlayerFourCard = playerFourCard, PlayerFiveCard = playerFiveCard, PlayerSixCard = playerSixCard };
                _db.Hands.Add(hand);
            }

            _db.SaveChanges();
        }

        public int TrickEnded(string gameID, string?[] played, char leadingSuit)
        {
            var score = _db.Scores.Where(x => x.GameID == gameID && x.PlayerOneScore != null).OrderByDescending(x => x.ID).First();
            var winningPlayer = FindWinningPlayer(played, leadingSuit, score.TrumpSuit);
            switch (winningPlayer)
            {
                case 0:
                    score.PlayerOneScore += 1;
                    break;
                case 1:
                    score.PlayerTwoScore += 1;
                    break;
                case 2:
                    score.PlayerThreeScore += 1;
                    break;
                case 3:
                    score.PlayerFourScore += 1;
                    break;
                case 4: 
                    score.PlayerFiveScore += 1;
                    break;
                case 5:
                    score.PlayerSixScore += 1;
                    break;
            }

            if (_db.ActiveHand.Where(x => x.GameID == gameID).Count() == _db.Hands.Where(x => x.GameID == gameID).Count())
            {
                RoundEnded(gameID, score);
                return -1;
            }

            return winningPlayer;
        }

        private void RoundEnded(string gameID, Scores score)
        {
            var bids = _db.Bids.Where(x => x.GameID == gameID).OrderByDescending(x => x.ID).First();

            if (bids.PlayerOneBid == score.PlayerOneScore)
            {
                score.PlayerOneScore += 10;
            }
            if (bids.PlayerTwoBid == score.PlayerTwoScore)
            {
                score.PlayerTwoScore += 10;
            }
            if (bids.PlayerThreeBid == score.PlayerThreeScore)
            {
                score.PlayerThreeScore += 10;
            }
            if (bids.PlayerFourBid == score.PlayerFourScore)
            {
                score.PlayerFourScore += 10;
            }
            if (bids.PlayerFiveBid == score.PlayerFiveScore)
            {
                score.PlayerFiveScore += 10;
            }
            if (bids.PlayerSixBid == score.PlayerSixScore)
            {
                score.PlayerSixScore += 10;
            }

            foreach (var row in _db.Hands.Where(x => x.GameID == gameID))
            {
                _db.Hands.Remove(row);
            }

            foreach (var row in _db.ActiveHand.Where(x => x.GameID == gameID))
            {
                _db.ActiveHand.Remove(row);
            }

            var numberOfRounds = _db.GameDetails.Where(x => x.GameID == gameID).First().NumberOfRounds;

            if (score.Round < numberOfRounds)
            {
                StartNextRound(gameID, score);
            }
            else
            {
                GameEnded(gameID);
            }
        }

        private void GameEnded(string gameID)
        {
            foreach (var row in _db.Bids.Where(x => x.GameID == gameID))
            {
                _db.Bids.Remove(row);
            }

            _db.GameDetails.Remove(_db.GameDetails.Where(x => x.GameID == gameID).First());
        }

        private void StartNextRound(string gameID, Scores score)
        {
            _db.Bids.Add(new Bids { GameID = gameID });

            var gameDetails = _db.GameDetails.Where(x => x.GameID == gameID).First();

            var nextRound = _db.Scores.Where(x => x.GameID == gameID && x.PlayerOneScore == null).OrderBy(x => x.ID).First();

            nextRound.PlayerOneScore = 0;
            nextRound.PlayerTwoScore = 0;
            if (gameDetails.NumberOfPlayers >= 3)
            {
                nextRound.PlayerThreeScore = 0;
            }
            if (gameDetails.NumberOfPlayers >= 4)
            {
                nextRound.PlayerFourScore = 0;
            }
            if (gameDetails.NumberOfPlayers >= 5)
            {
                nextRound.PlayerFiveScore = 0;
            }
            if (gameDetails.NumberOfPlayers == 6)
            {
                nextRound.PlayerSixScore = 0;
            }

            DealCards(gameID, nextRound.Tricks, gameDetails.NumberOfPlayers);

            _db.SaveChanges();

            _hub.Clients.Group(gameID).SendAsync("BidUpdate", score.Round % gameDetails.NumberOfPlayers);
        }

        private static int FindWinningPlayer(string?[] cards, char leading, char trump)
        {
            var winningCard = cards[0];

            var trumpCards = cards.Where(x => x?[1] == trump);
            if (trumpCards.Any())
            {
                winningCard = trumpCards.OrderByDescending(x => CardValue(x?[0])).First();
            } 
            else
            {
                winningCard = cards.Where(x => x?[1] == leading).OrderByDescending(x => CardValue(x?[0])).First();
            }

            var index = Array.IndexOf(cards, winningCard);
            return index;
        }

        private static int CardValue(char? card)
        {
            var value = card switch
            {
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                'T' => 10,
                'J' => 11,
                'Q' => 12,
                'K' => 13,
                'A' => 14,
                _ => throw new NotImplementedException()
            };

            return value;
        }
    }
}
