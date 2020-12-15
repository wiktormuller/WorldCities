using Microsoft.EntityFrameworkCore.Migrations;

namespace WorldCities.Migrations
{
    public partial class RepairnameofpropertyLatitude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lattitude",
                table: "Cities",
                newName: "Latitude");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Cities",
                newName: "Lattitude");
        }
    }
}
