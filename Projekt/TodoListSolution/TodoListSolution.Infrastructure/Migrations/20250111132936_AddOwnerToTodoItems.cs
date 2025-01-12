using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListSolution.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerToTodoItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "TodoItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "TodoItems");
        }
    }
}
