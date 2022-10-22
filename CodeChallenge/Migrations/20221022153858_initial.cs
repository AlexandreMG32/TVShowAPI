using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeChallenge.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "TVShows",
                columns: table => new
                {
                    TVShowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVShows", x => x.TVShowId);
                });

            migrationBuilder.CreateTable(
                name: "ActorTVShows",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    TVShowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorTVShows", x => new { x.ActorId, x.TVShowId });
                    table.ForeignKey(
                        name: "FK_ActorTVShows_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorTVShows_TVShows_TVShowId",
                        column: x => x.TVShowId,
                        principalTable: "TVShows",
                        principalColumn: "TVShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    EpisodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    TVShowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.EpisodeId);
                    table.ForeignKey(
                        name: "FK_Episodes_TVShows_TVShowId",
                        column: x => x.TVShowId,
                        principalTable: "TVShows",
                        principalColumn: "TVShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "ActorId", "BirthDate", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(1965, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jennifer Anniston" },
                    { 2, new DateTime(1968, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Courtney Cox" }
                });

            migrationBuilder.InsertData(
                table: "TVShows",
                columns: new[] { "TVShowId", "Description", "Genre", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "A comedy show", "Comedy", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Friends" },
                    { 2, "A small TV show", "Romance", new DateTime(1984, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Sopranos" }
                });

            migrationBuilder.InsertData(
                table: "ActorTVShows",
                columns: new[] { "ActorId", "TVShowId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Episodes",
                columns: new[] { "EpisodeId", "Description", "Duration", "TVShowId", "Title" },
                values: new object[,]
                {
                    { 1, "Test", 21.5, 1, "Pilot" },
                    { 2, "Test", 23.199999999999999, 2, "Pilot" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorTVShows_TVShowId",
                table: "ActorTVShows",
                column: "TVShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TVShowId",
                table: "Episodes",
                column: "TVShowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorTVShows");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "TVShows");
        }
    }
}
