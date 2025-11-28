using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliFHery.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddParcelPricingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DestinationPostalCode",
                schema: "delifhery",
                table: "delivery_orders",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DistanceSurcharge",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HeightCm",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LengthCm",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OriginPostalCode",
                schema: "delifhery",
                table: "delivery_orders",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SeasonalAdjustment",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightKg",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WidthCm",
                schema: "delifhery",
                table: "delivery_orders",
                type: "numeric(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                schema: "delifhery",
                table: "delivery_orders",
                keyColumn: "Id",
                keyValue: new Guid("95f44e9d-1c5a-4378-8f43-646f7414a87d"),
                columns: new[] { "BasePrice", "DestinationPostalCode", "DistanceSurcharge", "HeightCm", "LengthCm", "OriginPostalCode", "SeasonalAdjustment", "TotalPrice", "WeightKg", "WidthCm" },
                values: new object[] { 10m, "94107", 2.5m, 10m, 30m, "94103", 0m, 12.5m, 2.5m, 20m });

            migrationBuilder.UpdateData(
                schema: "delifhery",
                table: "delivery_orders",
                keyColumn: "Id",
                keyValue: new Guid("d882e388-611d-49c2-9d2e-5a8bf9fa33e5"),
                columns: new[] { "BasePrice", "DestinationPostalCode", "DistanceSurcharge", "HeightCm", "LengthCm", "OriginPostalCode", "SeasonalAdjustment", "TotalPrice", "WeightKg", "WidthCm" },
                values: new object[] { 15m, "94501", 7.5m, 15m, 40m, "94016", 0m, 22.5m, 6.2m, 25m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePrice",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "DestinationPostalCode",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "DistanceSurcharge",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "HeightCm",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "LengthCm",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "OriginPostalCode",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "SeasonalAdjustment",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                schema: "delifhery",
                table: "delivery_orders");

            migrationBuilder.DropColumn(
                name: "WidthCm",
                schema: "delifhery",
                table: "delivery_orders");
        }
    }
}
