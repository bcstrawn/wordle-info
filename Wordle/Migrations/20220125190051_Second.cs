using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wordle.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Guesses",
                table: "Guesses");

            migrationBuilder.RenameTable(
                name: "Guesses",
                newName: "Guess");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guess",
                table: "Guess",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Guess",
                table: "Guess");

            migrationBuilder.RenameTable(
                name: "Guess",
                newName: "Guesses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guesses",
                table: "Guesses",
                column: "Id");
        }
    }
}
