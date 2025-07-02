using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CattleBreedsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightToPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "CattlePredictions",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CattlePredictions");
        }
    }
}
