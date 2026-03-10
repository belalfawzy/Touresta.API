using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RAFIQ.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHelperOperationalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BanReason",
                table: "Helpers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedAt",
                table: "Helpers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BannedByAdminId",
                table: "Helpers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Helpers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "Helpers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                table: "Helpers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuspendedByAdminId",
                table: "Helpers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspensionReason",
                table: "Helpers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_ApprovalStatus",
                table: "Helpers",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_IsActive",
                table: "Helpers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_IsApproved",
                table: "Helpers",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_IsBanned",
                table: "Helpers",
                column: "IsBanned");

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_IsSuspended",
                table: "Helpers",
                column: "IsSuspended");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Helpers_ApprovalStatus",
                table: "Helpers");

            migrationBuilder.DropIndex(
                name: "IX_Helpers_IsActive",
                table: "Helpers");

            migrationBuilder.DropIndex(
                name: "IX_Helpers_IsApproved",
                table: "Helpers");

            migrationBuilder.DropIndex(
                name: "IX_Helpers_IsBanned",
                table: "Helpers");

            migrationBuilder.DropIndex(
                name: "IX_Helpers_IsSuspended",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "BanReason",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "BannedAt",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "BannedByAdminId",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "SuspendedByAdminId",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "SuspensionReason",
                table: "Helpers");
        }
    }
}
