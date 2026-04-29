using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("021562af-256f-4add-9199-5cb733b25740"));

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("258d9a72-443f-4dc6-a3b0-497c5086568f"));

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("5acc87df-be78-4713-9012-02adef6d70b1"));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TaskTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "IsDeleted", "TagId", "TaskId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("43539ea7-8674-4478-b6ee-d81e0eb10831"), new DateTime(2026, 4, 29, 15, 14, 48, 837, DateTimeKind.Utc).AddTicks(5998), null, null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("a3ccfb17-4b4d-46e2-874a-37f60df9a7b8"), new DateTime(2026, 4, 29, 15, 14, 48, 838, DateTimeKind.Utc).AddTicks(3475), null, null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("40000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("b6af2800-701d-4d0a-8958-bcf2ee56524c"), new DateTime(2026, 4, 29, 15, 14, 48, 838, DateTimeKind.Utc).AddTicks(3441), null, null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000002"), null, null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "", "" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("43539ea7-8674-4478-b6ee-d81e0eb10831"));

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("a3ccfb17-4b4d-46e2-874a-37f60df9a7b8"));

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("b6af2800-701d-4d0a-8958-bcf2ee56524c"));

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.InsertData(
                table: "TaskTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "IsDeleted", "TagId", "TaskId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("021562af-256f-4add-9199-5cb733b25740"), new DateTime(2026, 4, 29, 14, 45, 16, 964, DateTimeKind.Utc).AddTicks(9028), null, null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("258d9a72-443f-4dc6-a3b0-497c5086568f"), new DateTime(2026, 4, 29, 14, 45, 16, 965, DateTimeKind.Utc).AddTicks(1327), null, null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("5acc87df-be78-4713-9012-02adef6d70b1"), new DateTime(2026, 4, 29, 14, 45, 16, 965, DateTimeKind.Utc).AddTicks(1339), null, null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("40000000-0000-0000-0000-000000000002"), null, null }
                });
        }
    }
}
