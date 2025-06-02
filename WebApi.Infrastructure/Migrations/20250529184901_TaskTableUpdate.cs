using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaskTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isBlocked",
                table: "users",
                newName: "IsBlocked");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "tasks");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "users",
                newName: "isBlocked");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "tasks",
                type: "bytea",
                nullable: true);
        }
    }
}
