using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendRequestWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friends_friend_id",
                table: "Friends");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "Friends",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "responded_at",
                table: "Friends",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Friends_friend_id_status",
                table: "Friends",
                columns: new[] { "friend_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Friends_friend_id_status",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "responded_at",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Friends");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_friend_id",
                table: "Friends",
                column: "friend_id");
        }
    }
}
