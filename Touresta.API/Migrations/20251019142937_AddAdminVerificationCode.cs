using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Touresta.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminVerificationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleId",
                table: "Admins",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "Admins",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationCodeExpiry",
                table: "Admins",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "VerificationCodeExpiry",
                table: "Admins");
        }
    }
}
