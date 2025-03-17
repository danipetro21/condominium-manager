using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace cem.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Condominiums",
                columns: new[] { "Id", "Address", "City", "CreationDate", "Name", "PostalCode", "Province" },
                values: new object[,]
                {
                    { 1, "Via Roma 1", "Milano", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Condominio A", "20100", "MI" },
                    { 2, "Via Dante 2", "Torino", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Condominio B", "10100", "TO" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", "Admin", "User", "hashed_password_here", 0, "admin" },
                    { 2, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "manager1@example.com", "Manager", "One", "hashed_password_here", 1, "manager1" }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "ApprovedAt", "ApprovedById", "AttachmentPath", "CondominiumId", "CreatedAt", "CreatedById", "Date", "Description", "RejectionReason", "Status", "Type" },
                values: new object[] { 1, 100.50m, null, null, null, 1, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pulizia scale", null, 0, "Manutenzione" });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "ExpenseId", "Message", "ReadAt", "Title", "Type", "UserId" },
                values: new object[] { 1, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Una nuova spesa è stata creata.", null, "Nuova spesa", 0, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Condominiums",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Condominiums",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
