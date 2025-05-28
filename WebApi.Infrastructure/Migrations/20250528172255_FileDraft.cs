using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FileDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "tasks");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "tasks",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "tasks");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "tasks",
                type: "bytea",
                nullable: true);
        }
    }
}
