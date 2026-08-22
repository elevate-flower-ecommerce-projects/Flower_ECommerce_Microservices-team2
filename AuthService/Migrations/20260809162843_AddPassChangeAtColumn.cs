using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class AddPassChangeAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'PasswordChangedAt'
                )
                BEGIN
                    ALTER TABLE [Users] ADD [PasswordChangedAt] datetime2 NULL;
                END
            ");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 203, DateTimeKind.Utc).AddTicks(9440));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 204, DateTimeKind.Utc).AddTicks(1069));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 204, DateTimeKind.Utc).AddTicks(1071));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 206, DateTimeKind.Utc).AddTicks(979));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 206, DateTimeKind.Utc).AddTicks(2179));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 206, DateTimeKind.Utc).AddTicks(2181));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 4L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 9, 16, 28, 42, 206, DateTimeKind.Utc).AddTicks(2183));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'PasswordChangedAt'
                )
                BEGIN
                    ALTER TABLE [Users] DROP COLUMN [PasswordChangedAt];
                END
            ");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 751, DateTimeKind.Utc).AddTicks(3754));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 751, DateTimeKind.Utc).AddTicks(4581));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 751, DateTimeKind.Utc).AddTicks(4583));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 752, DateTimeKind.Utc).AddTicks(7448));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 752, DateTimeKind.Utc).AddTicks(8072));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 3L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 752, DateTimeKind.Utc).AddTicks(8074));

            migrationBuilder.UpdateData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 4L,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 7, 23, 15, 50, 752, DateTimeKind.Utc).AddTicks(8075));
        }
    }
}
