using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlatform.Order.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IdempotentColumCompositeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderOutboxes",
                table: "OrderOutboxes");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OrderOutboxes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OrderOutboxes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderOutboxes",
                table: "OrderOutboxes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOutboxes_IdempotentToken_Type",
                table: "OrderOutboxes",
                columns: new[] { "IdempotentToken", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderOutboxes",
                table: "OrderOutboxes");

            migrationBuilder.DropIndex(
                name: "IX_OrderOutboxes_IdempotentToken_Type",
                table: "OrderOutboxes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderOutboxes");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OrderOutboxes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderOutboxes",
                table: "OrderOutboxes",
                column: "IdempotentToken");
        }
    }
}
