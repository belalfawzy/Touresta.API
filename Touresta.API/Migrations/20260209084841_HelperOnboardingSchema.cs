using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Touresta.API.Migrations
{
    /// <inheritdoc />
    public partial class HelperOnboardingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrugTestFile",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Helpers");

            migrationBuilder.RenameColumn(
                name: "DrugTestExpiry",
                table: "Helpers",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "NationalIdPhoto",
                table: "Helpers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Helpers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CriminalRecordFile",
                table: "Helpers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HelperId",
                table: "Helpers",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Helpers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Helpers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Helpers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Helpers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByAdminId",
                table: "Helpers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Helpers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Helpers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Certificates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Cars",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdminAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetType = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAuditLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DrugTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HelperId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsCurrent = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugTests_Helpers_HelperId",
                        column: x => x.HelperId,
                        principalTable: "Helpers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HelperLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HelperId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LanguageName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    AiScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    TestAttempts = table.Column<int>(type: "int", nullable: false),
                    LastTestedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsVerified = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelperLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelperLanguages_Helpers_HelperId",
                        column: x => x.HelperId,
                        principalTable: "Helpers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HelperLanguageId = table.Column<int>(type: "int", nullable: false),
                    AiScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AiLevel = table.Column<int>(type: "int", nullable: false),
                    Passed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TakenAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageTests_HelperLanguages_HelperLanguageId",
                        column: x => x.HelperLanguageId,
                        principalTable: "HelperLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_HelperId",
                table: "Helpers",
                column: "HelperId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Helpers_UserId",
                table: "Helpers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_LicensePlate",
                table: "Cars",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditLogs_AdminId",
                table: "AdminAuditLogs",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditLogs_TargetType_TargetId",
                table: "AdminAuditLogs",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditLogs_Timestamp",
                table: "AdminAuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DrugTests_HelperId_IsCurrent",
                table: "DrugTests",
                columns: new[] { "HelperId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_HelperLanguages_HelperId_LanguageCode",
                table: "HelperLanguages",
                columns: new[] { "HelperId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageTests_HelperLanguageId",
                table: "LanguageTests",
                column: "HelperLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Helpers_Users_UserId",
                table: "Helpers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Helpers_Users_UserId",
                table: "Helpers");

            migrationBuilder.DropTable(
                name: "AdminAuditLogs");

            migrationBuilder.DropTable(
                name: "DrugTests");

            migrationBuilder.DropTable(
                name: "LanguageTests");

            migrationBuilder.DropTable(
                name: "HelperLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Helpers_HelperId",
                table: "Helpers");

            migrationBuilder.DropIndex(
                name: "IX_Helpers_UserId",
                table: "Helpers");

            migrationBuilder.DropIndex(
                name: "IX_Cars_LicensePlate",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "CriminalRecordFile",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "HelperId",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "ReviewedByAdminId",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Helpers");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Helpers",
                newName: "DrugTestExpiry");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Helpers",
                keyColumn: "NationalIdPhoto",
                keyValue: null,
                column: "NationalIdPhoto",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "NationalIdPhoto",
                table: "Helpers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DrugTestFile",
                table: "Helpers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Helpers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Helpers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Helpers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Helpers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Cars",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
