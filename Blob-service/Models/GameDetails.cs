namespace Blob_service.Models
{
    public class GameDetails
    {
        public int ID { get; set; }

        public string GameID { get; set; }

        public int NumberOfPlayers { get; set; }

        public int NumberOfRounds { get; set; }

        public bool NoTrumpsRound { get; set; }

        public bool ScoreOnMakingBidOnly { get; set; }

        public int BotsPositions { get; set; }
    }
}
