using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "test_tasks");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "completed_tasks");

            migrationBuilder.AddColumn<bool>(
                name: "isBlocked",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "completed_tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBlocked",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "completed_tasks");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "test_tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "completed_tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
