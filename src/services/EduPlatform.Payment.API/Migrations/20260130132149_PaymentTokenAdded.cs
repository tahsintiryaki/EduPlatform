using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlatform.Payment.API.Migrations
{
    /// <inheritdoc />
    public partial class PaymentTokenAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentToken",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentToken",
                table: "Payments");
        }
    }
}
