using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyRegistration",
                table: "Invoices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Invoices",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "Invoices",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Invoices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyRegistration",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "LegalAddress",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Invoices");
        }
    }
}
