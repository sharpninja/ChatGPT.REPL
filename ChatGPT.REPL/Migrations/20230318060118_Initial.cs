using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatGPT.REPL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromptResponses",
                columns: table => new
                {
                    UID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", nullable: true),
                    Response = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptResponses", x => x.UID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromptResponses");
        }
    }
}
