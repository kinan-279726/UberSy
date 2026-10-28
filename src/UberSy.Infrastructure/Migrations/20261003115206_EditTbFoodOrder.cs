using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UberSy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditTbFoodOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResponseTimeout",
                table: "TbFoodOrder",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseTimeout",
                table: "TbFoodOrder");
        }
    }
}
