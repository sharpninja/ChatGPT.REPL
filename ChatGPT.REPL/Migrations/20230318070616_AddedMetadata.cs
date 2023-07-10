using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatGPT.REPL.Migrations
{
    /// <inheritdoc />
    public partial class AddedMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionName",
                table: "PromptResponses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "PromptResponses",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionName",
                table: "PromptResponses");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "PromptResponses");
        }
    }
}
