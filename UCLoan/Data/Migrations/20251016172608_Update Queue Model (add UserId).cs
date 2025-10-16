using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCLoan.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQueueModeladdUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queue_AspNetUsers_UserId",
                table: "Queue");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Queue",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Queue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_AspNetUsers_UserId",
                table: "Queue",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queue_AspNetUsers_UserId",
                table: "Queue");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Queue");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Queue",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_AspNetUsers_UserId",
                table: "Queue",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
