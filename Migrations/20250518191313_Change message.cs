using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendTagBackend.Migrations
{
    /// <inheritdoc />
    public partial class Changemessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Messages",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Messages",
                type: "double",
                nullable: true);
        }
    }
}
