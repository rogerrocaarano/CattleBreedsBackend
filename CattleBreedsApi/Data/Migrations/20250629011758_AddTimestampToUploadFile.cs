﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CattleBreedsApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampToUploadFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "UploadFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "UploadFiles");
        }
    }
}
