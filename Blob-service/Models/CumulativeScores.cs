namespace Blob_service.Models
{
    public class CumulativeScores
    {
        public int Tricks { get; set; }

        public char TrumpSuit { get; set; }

        public int? PlayerOneScore { get; set; }

        public int? PlayerOneCumulativeScore { get; set; }

        public int? PlayerTwoScore { get; set; }

        public int? PlayerTwoCumulativeScore { get; set; }

        public int? PlayerThreeScore { get; set; }

        public int? PlayerThreeCumulativeScore { get; set; }

        public int? PlayerFourScore { get; set; }

        public int? PlayerFourCumulativeScore { get; set; }

        public int? PlayerFiveScore { get; set; }

        public int? PlayerFiveCumulativeScore { get; set; }

        public int? PlayerSixScore { get; set; }

        public int? PlayerSixCumulativeScore {  get; set; }
    }
}
