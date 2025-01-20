using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeerStore.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryAndBeerModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Countries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BeerId",
                table: "Beers",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Countries",
                newName: "CountryId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Beers",
                newName: "BeerId");
        }
    }
}
