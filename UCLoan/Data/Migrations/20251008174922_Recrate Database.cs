using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCLoan.Data.Migrations
{
    /// <inheritdoc />
    public partial class RecrateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentId",
                table: "Equipment",
                column: "EquipmentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Equipment_EquipmentId",
                table: "Equipment");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
