using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DeliFHery.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "delifhery");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "delifhery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                schema: "delifhery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacts_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "delifhery",
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "delivery_orders",
                schema: "delifhery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_delivery_orders_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "delifhery",
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "delifhery",
                table: "customers",
                columns: new[] { "Id", "City", "Name", "Notes", "PostalCode", "Street" },
                values: new object[,]
                {
                    { new Guid("5a6b9e21-7c74-4d3b-8a6a-1ef65c94af10"), "Springfield", "Orchard Market", "Prefers early deliveries", "10001", "Sunrise Blvd 10" },
                    { new Guid("dbcd7f5b-7e9e-4c8f-9a2c-6f2b2e4c60f7"), "Springfield", "City Bakery", "Call ahead when delayed", "10002", "Downtown Ave 5" }
                });

            migrationBuilder.InsertData(
                schema: "delifhery",
                table: "contacts",
                columns: new[] { "Id", "CustomerId", "IsPrimary", "Type", "Value" },
                values: new object[,]
                {
                    { new Guid("6ed5825d-45df-4f23-9bd7-dfa48bb3cdb8"), new Guid("5a6b9e21-7c74-4d3b-8a6a-1ef65c94af10"), true, "Email", "orders@orchard-market.example" },
                    { new Guid("85fe89b7-8ac8-42ba-9013-38f5fe403fb0"), new Guid("5a6b9e21-7c74-4d3b-8a6a-1ef65c94af10"), false, "Phone", "+1-800-555-1234" },
                    { new Guid("df1a0a3c-92fd-4f0c-a341-1af7b7f33d72"), new Guid("dbcd7f5b-7e9e-4c8f-9a2c-6f2b2e4c60f7"), true, "Phone", "+1-800-555-9876" }
                });

            migrationBuilder.InsertData(
                schema: "delifhery",
                table: "delivery_orders",
                columns: new[] { "Id", "CustomerId", "DeliveredAt", "OrderNumber", "ScheduledAt", "Status" },
                values: new object[,]
                {
                    { new Guid("95f44e9d-1c5a-4378-8f43-646f7414a87d"), new Guid("5a6b9e21-7c74-4d3b-8a6a-1ef65c94af10"), null, "ORD-1000", new DateTimeOffset(new DateTime(2024, 4, 10, 7, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Created" },
                    { new Guid("d882e388-611d-49c2-9d2e-5a8bf9fa33e5"), new Guid("dbcd7f5b-7e9e-4c8f-9a2c-6f2b2e4c60f7"), null, "ORD-2000", new DateTimeOffset(new DateTime(2024, 4, 10, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Created" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_CustomerId",
                schema: "delifhery",
                table: "contacts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_orders_CustomerId",
                schema: "delifhery",
                table: "delivery_orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_orders_OrderNumber",
                schema: "delifhery",
                table: "delivery_orders",
                column: "OrderNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacts",
                schema: "delifhery");

            migrationBuilder.DropTable(
                name: "delivery_orders",
                schema: "delifhery");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "delifhery");
        }
    }
}
