namespace Blob_service.Models
{
    public class Scores
    {
        public int ID { get; set; }

        public int GameID { get; set; }

        public int Round { get; set; }

        public int Tricks { get; set; }

        public char TrumpSuit { get; set; }

        public int? PlayerOneScore { get; set; }

        public int? PlayerTwoScore { get; set; }

        public int? PlayerThreeScore { get; set; }

        public int? PlayerFourScore { get; set; }

        public int? PlayerFiveScore { get; set; }

        public int? PlayerSixScore { get; set; }
    }
}
