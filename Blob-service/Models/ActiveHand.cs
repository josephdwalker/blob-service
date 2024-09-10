namespace Blob_service.Models
{
    public class ActiveHand
    {
        public int ID { get; set; }

        public int GameID { get; set; }

        public char LeadingSuit { get; set; }

        public string? PlayerOneCard { get; set; }

        public string? PlayerTwoCard { get; set; }

        public string? PlayerThreeCard { get; set; }

        public string? PlayerFourCard { get; set; }

        public string? PlayerFiveCard { get; set; }

        public string? PlayerSixCard { get; set; }
    }
}
