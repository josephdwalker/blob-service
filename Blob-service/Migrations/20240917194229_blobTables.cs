using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blob_service.Migrations
{
    /// <inheritdoc />
    public partial class blobTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveHand",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeadingSuit = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    PlayerOneCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerTwoCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerThreeCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerFourCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerFiveCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerSixCard = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveHand", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerOneBid = table.Column<int>(type: "int", nullable: true),
                    PlayerTwoBid = table.Column<int>(type: "int", nullable: true),
                    PlayerThreeBid = table.Column<int>(type: "int", nullable: true),
                    PlayerFourBid = table.Column<int>(type: "int", nullable: true),
                    PlayerFiveBid = table.Column<int>(type: "int", nullable: true),
                    PlayerSixBid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GameDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfPlayers = table.Column<int>(type: "int", nullable: false),
                    NumberOfRounds = table.Column<int>(type: "int", nullable: false),
                    NoTrumpsRound = table.Column<bool>(type: "bit", nullable: false),
                    ScoreOnMakingBidOnly = table.Column<bool>(type: "bit", nullable: false),
                    BotsPositions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Hands",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerOneCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerTwoCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerThreeCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerFourCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerFiveCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerSixCard = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hands", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    Tricks = table.Column<int>(type: "int", nullable: false),
                    TrumpSuit = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    PlayerOneScore = table.Column<int>(type: "int", nullable: true),
                    PlayerTwoScore = table.Column<int>(type: "int", nullable: true),
                    PlayerThreeScore = table.Column<int>(type: "int", nullable: true),
                    PlayerFourScore = table.Column<int>(type: "int", nullable: true),
                    PlayerFiveScore = table.Column<int>(type: "int", nullable: true),
                    PlayerSixScore = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveHand");

            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "GameDetails");

            migrationBuilder.DropTable(
                name: "Hands");

            migrationBuilder.DropTable(
                name: "Scores");
        }
    }
}
