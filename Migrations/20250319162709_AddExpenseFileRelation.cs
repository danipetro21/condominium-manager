using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cem.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseFileRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Expenses_ExpenseId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ExpenseId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ExpenseId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Files_EntityId",
                table: "Files",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Expenses_EntityId",
                table: "Files",
                column: "EntityId",
                principalTable: "Expenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Expenses_EntityId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_EntityId",
                table: "Files");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ExpenseId",
                table: "Files",
                column: "ExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Expenses_ExpenseId",
                table: "Files",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id");
        }
    }
}
