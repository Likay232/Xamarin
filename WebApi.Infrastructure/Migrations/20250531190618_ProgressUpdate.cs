using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProgressUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountToLevelUp",
                table: "progresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountToLevelUp",
                table: "progresses");
        }
    }
}
