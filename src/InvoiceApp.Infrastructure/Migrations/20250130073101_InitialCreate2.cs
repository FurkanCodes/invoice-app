using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EmailVerifications");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "EmailVerifications");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "EmailVerifications");

            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "EmailVerifications");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "EmailVerifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "EmailVerifications");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EmailVerifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "EmailVerifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "EmailVerifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "EmailVerifications",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
