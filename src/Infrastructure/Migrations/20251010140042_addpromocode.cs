using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addpromocode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "PromoCodes",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "PromoCodes",
                newName: "MinimumOrderAmount");

            migrationBuilder.AddColumn<int>(
                name: "CurrentUsageCount",
                table: "PromoCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "PromoCodes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "PromoCodes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsageCount",
                table: "PromoCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "PromoCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentUsageCount",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "MaxUsageCount",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "PromoCodes");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "PromoCodes",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "MinimumOrderAmount",
                table: "PromoCodes",
                newName: "DiscountValue");
        }
    }
}
