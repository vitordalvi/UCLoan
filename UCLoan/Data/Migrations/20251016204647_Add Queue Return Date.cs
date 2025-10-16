using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCLoan.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQueueReturnDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "Queue",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "Queue");
        }
    }
}
