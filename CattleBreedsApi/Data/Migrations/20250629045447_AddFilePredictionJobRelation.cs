using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CattleBreedsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePredictionJobRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BestResultImageId",
                table: "CattlePredictionJobs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CattlePredictionJobs_BestResultImageId",
                table: "CattlePredictionJobs",
                column: "BestResultImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CattlePredictionJobs_UploadFiles_BestResultImageId",
                table: "CattlePredictionJobs",
                column: "BestResultImageId",
                principalTable: "UploadFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CattlePredictionJobs_UploadFiles_BestResultImageId",
                table: "CattlePredictionJobs");

            migrationBuilder.DropIndex(
                name: "IX_CattlePredictionJobs_BestResultImageId",
                table: "CattlePredictionJobs");

            migrationBuilder.DropColumn(
                name: "BestResultImageId",
                table: "CattlePredictionJobs");
        }
    }
}
