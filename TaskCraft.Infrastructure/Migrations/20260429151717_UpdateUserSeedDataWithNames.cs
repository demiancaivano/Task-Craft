using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSeedDataWithNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "TaskTags",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "IsDeleted", "TagId", "TaskId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("ac1a0737-a795-47f7-94a3-aa9905bbc526"), new DateTime(2026, 4, 29, 15, 17, 12, 910, DateTimeKind.Utc).AddTicks(1563), null, null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("ae6769d1-d838-4abe-9477-60f29cb2c887"), new DateTime(2026, 4, 29, 15, 17, 12, 909, DateTimeKind.Utc).AddTicks(2339), null, null, false, new Guid("30000000-0000-0000-0000-000000000002"), new Guid("40000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("c068aa69-0c62-491f-b782-363194bfe5b1"), new DateTime(2026, 4, 29, 15, 17, 12, 910, DateTimeKind.Utc).AddTicks(1601), null, null, false, new Guid("30000000-0000-0000-0000-000000000004"), new Guid("40000000-0000-0000-0000-000000000002"), null, null }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Admin", "User" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "John", "Doe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Jane", "Doe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("ac1a0737-a795-47f7-94a3-aa9905bbc526"));

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("ae6769d1-d838-4abe-9477-60f29cb2c887"));

            migrationBuilder.DeleteData(
                table: "TaskTags",
                keyColumn: "Id",
                keyValue: new Guid("c068aa69-0c62-491f-b782-363194bfe5b1"));

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
        }
    }
}
