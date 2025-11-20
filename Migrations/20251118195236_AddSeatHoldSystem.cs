using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoCylone.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatHoldSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HoldExpiryTime",
                table: "BookingSeats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessingStartTime",
                table: "BookingSeats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BookingSeats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoldExpiryTime",
                table: "BookingSeats");

            migrationBuilder.DropColumn(
                name: "ProcessingStartTime",
                table: "BookingSeats");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookingSeats");
        }
    }
}
