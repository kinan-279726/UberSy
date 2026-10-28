using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UberSy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditTbOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResponseTimeout",
                table: "TbServiceRequestOffer",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "TbServiceRequestOffer",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseTimeout",
                table: "TbServiceRequestOffer");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "TbServiceRequestOffer");
        }
    }
}
