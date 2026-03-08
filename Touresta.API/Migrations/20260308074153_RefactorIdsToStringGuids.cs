using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Touresta.API.Migrations
{
    /// <inheritdoc />
    public partial class RefactorIdsToStringGuids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(name: "FK_HelperReports_Users_UserId", table: "HelperReports");
            migrationBuilder.DropForeignKey(name: "FK_HelperReports_Helpers_HelperId", table: "HelperReports");
            migrationBuilder.DropForeignKey(name: "FK_AdminNotes_Helpers_HelperId", table: "AdminNotes");
            migrationBuilder.DropForeignKey(name: "FK_LanguageTests_HelperLanguages_HelperLanguageId", table: "LanguageTests");
            migrationBuilder.DropForeignKey(name: "FK_DrugTests_Helpers_HelperId", table: "DrugTests");
            migrationBuilder.DropForeignKey(name: "FK_Certificates_Helpers_HelperId", table: "Certificates");
            migrationBuilder.DropForeignKey(name: "FK_HelperLanguages_Helpers_HelperId", table: "HelperLanguages");
            migrationBuilder.DropForeignKey(name: "FK_Cars_Helpers_HelperId", table: "Cars");
            migrationBuilder.DropForeignKey(name: "FK_Helpers_Users_UserId", table: "Helpers");

           
            // Primary keys
            migrationBuilder.Sql("UPDATE `Users` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Admins` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Helpers` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Cars` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Certificates` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `HelperLanguages` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `LanguageTests` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `DrugTests` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `AdminAuditLogs` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `AdminNotes` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `HelperReports` SET `Id` = REPLACE(`Id`, '-', '') WHERE `Id` LIKE '%-%';");

            // Foreign key columns
            migrationBuilder.Sql("UPDATE `Helpers` SET `UserId` = REPLACE(`UserId`, '-', '') WHERE `UserId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Helpers` SET `BannedByAdminId` = REPLACE(`BannedByAdminId`, '-', '') WHERE `BannedByAdminId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Helpers` SET `SuspendedByAdminId` = REPLACE(`SuspendedByAdminId`, '-', '') WHERE `SuspendedByAdminId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Helpers` SET `ReviewedByAdminId` = REPLACE(`ReviewedByAdminId`, '-', '') WHERE `ReviewedByAdminId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Cars` SET `HelperId` = REPLACE(`HelperId`, '-', '') WHERE `HelperId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `Certificates` SET `HelperId` = REPLACE(`HelperId`, '-', '') WHERE `HelperId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `HelperLanguages` SET `HelperId` = REPLACE(`HelperId`, '-', '') WHERE `HelperId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `LanguageTests` SET `HelperLanguageId` = REPLACE(`HelperLanguageId`, '-', '') WHERE `HelperLanguageId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `DrugTests` SET `HelperId` = REPLACE(`HelperId`, '-', '') WHERE `HelperId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `AdminAuditLogs` SET `AdminId` = REPLACE(`AdminId`, '-', '') WHERE `AdminId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `AdminAuditLogs` SET `TargetId` = REPLACE(`TargetId`, '-', '') WHERE `TargetId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `HelperReports` SET `HelperId` = REPLACE(`HelperId`, '-', '') WHERE `HelperId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `HelperReports` SET `UserId` = REPLACE(`UserId`, '-', '') WHERE `UserId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `HelperReports` SET `ResolvedByAdminId` = REPLACE(`ResolvedByAdminId`, '-', '') WHERE `ResolvedByAdminId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `AdminNotes` SET `HelperId` = REPLACE(`HelperId`, '-', '') WHERE `HelperId` LIKE '%-%';");
            migrationBuilder.Sql("UPDATE `AdminNotes` SET `AdminId` = REPLACE(`AdminId`, '-', '') WHERE `AdminId` LIKE '%-%';");


            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "Users",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "Admins",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "Helpers",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "Cars",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "Certificates",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "HelperLanguages",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "LanguageTests",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "DrugTests",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "AdminAuditLogs",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "AdminNotes",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Id", table: "HelperReports",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);


            // Helpers FK columns
            migrationBuilder.AlterColumn<string>(
                name: "UserId", table: "Helpers",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "BannedByAdminId", table: "Helpers",
                type: "varchar(32)", maxLength: 32, nullable: true,
                oldClrType: typeof(int), oldType: "int", oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SuspendedByAdminId", table: "Helpers",
                type: "varchar(32)", maxLength: 32, nullable: true,
                oldClrType: typeof(int), oldType: "int", oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewedByAdminId", table: "Helpers",
                type: "varchar(32)", maxLength: 32, nullable: true,
                oldClrType: typeof(int), oldType: "int", oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Cars FK
            migrationBuilder.AlterColumn<string>(
                name: "HelperId", table: "Cars",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // Certificates FK
            migrationBuilder.AlterColumn<string>(
                name: "HelperId", table: "Certificates",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // HelperLanguages FK
            migrationBuilder.AlterColumn<string>(
                name: "HelperId", table: "HelperLanguages",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // LanguageTests FK
            migrationBuilder.AlterColumn<string>(
                name: "HelperLanguageId", table: "LanguageTests",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // DrugTests FK
            migrationBuilder.AlterColumn<string>(
                name: "HelperId", table: "DrugTests",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // AdminAuditLogs (no FK constraint but still need type change)
            migrationBuilder.AlterColumn<string>(
                name: "AdminId", table: "AdminAuditLogs",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TargetId", table: "AdminAuditLogs",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            // HelperReports FK columns
            migrationBuilder.AlterColumn<string>(
                name: "HelperId", table: "HelperReports",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId", table: "HelperReports",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ResolvedByAdminId", table: "HelperReports",
                type: "varchar(32)", maxLength: 32, nullable: true,
                oldClrType: typeof(int), oldType: "int", oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // AdminNotes FK columns
            migrationBuilder.AlterColumn<string>(
                name: "HelperId", table: "AdminNotes",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId", table: "AdminNotes",
                type: "varchar(32)", maxLength: 32, nullable: false,
                oldClrType: typeof(int), oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");


            migrationBuilder.AddForeignKey(
                name: "FK_Helpers_Users_UserId",
                table: "Helpers", column: "UserId",
                principalTable: "Users", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Helpers_HelperId",
                table: "Cars", column: "HelperId",
                principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Helpers_HelperId",
                table: "Certificates", column: "HelperId",
                principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperLanguages_Helpers_HelperId",
                table: "HelperLanguages", column: "HelperId",
                principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguageTests_HelperLanguages_HelperLanguageId",
                table: "LanguageTests", column: "HelperLanguageId",
                principalTable: "HelperLanguages", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DrugTests_Helpers_HelperId",
                table: "DrugTests", column: "HelperId",
                principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperReports_Helpers_HelperId",
                table: "HelperReports", column: "HelperId",
                principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperReports_Users_UserId",
                table: "HelperReports", column: "UserId",
                principalTable: "Users", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdminNotes_Helpers_HelperId",
                table: "AdminNotes", column: "HelperId",
                principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_HelperReports_Users_UserId", table: "HelperReports");
            migrationBuilder.DropForeignKey(name: "FK_HelperReports_Helpers_HelperId", table: "HelperReports");
            migrationBuilder.DropForeignKey(name: "FK_AdminNotes_Helpers_HelperId", table: "AdminNotes");
            migrationBuilder.DropForeignKey(name: "FK_LanguageTests_HelperLanguages_HelperLanguageId", table: "LanguageTests");
            migrationBuilder.DropForeignKey(name: "FK_DrugTests_Helpers_HelperId", table: "DrugTests");
            migrationBuilder.DropForeignKey(name: "FK_Certificates_Helpers_HelperId", table: "Certificates");
            migrationBuilder.DropForeignKey(name: "FK_HelperLanguages_Helpers_HelperId", table: "HelperLanguages");
            migrationBuilder.DropForeignKey(name: "FK_Cars_Helpers_HelperId", table: "Cars");
            migrationBuilder.DropForeignKey(name: "FK_Helpers_Users_UserId", table: "Helpers");


            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "Users", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "Admins", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "Helpers", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "Cars", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "Certificates", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "HelperLanguages", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "LanguageTests", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "DrugTests", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "AdminAuditLogs", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "AdminNotes", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id", table: "HelperReports", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");


            migrationBuilder.AlterColumn<int>(
                name: "UserId", table: "Helpers", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "BannedByAdminId", table: "Helpers", type: "int", nullable: true,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32, oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "SuspendedByAdminId", table: "Helpers", type: "int", nullable: true,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32, oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewedByAdminId", table: "Helpers", type: "int", nullable: true,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32, oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperId", table: "Cars", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperId", table: "Certificates", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperId", table: "HelperLanguages", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperLanguageId", table: "LanguageTests", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperId", table: "DrugTests", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId", table: "AdminAuditLogs", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TargetId", table: "AdminAuditLogs", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperId", table: "HelperReports", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "UserId", table: "HelperReports", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ResolvedByAdminId", table: "HelperReports", type: "int", nullable: true,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32, oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "HelperId", table: "AdminNotes", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId", table: "AdminNotes", type: "int", nullable: false,
                oldClrType: typeof(string), oldType: "varchar(32)", oldMaxLength: 32)
                .OldAnnotation("MySql:CharSet", "utf8mb4");


            migrationBuilder.AddForeignKey(
                name: "FK_Helpers_Users_UserId", table: "Helpers",
                column: "UserId", principalTable: "Users", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Helpers_HelperId", table: "Cars",
                column: "HelperId", principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Helpers_HelperId", table: "Certificates",
                column: "HelperId", principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperLanguages_Helpers_HelperId", table: "HelperLanguages",
                column: "HelperId", principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguageTests_HelperLanguages_HelperLanguageId", table: "LanguageTests",
                column: "HelperLanguageId", principalTable: "HelperLanguages", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DrugTests_Helpers_HelperId", table: "DrugTests",
                column: "HelperId", principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperReports_Helpers_HelperId", table: "HelperReports",
                column: "HelperId", principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperReports_Users_UserId", table: "HelperReports",
                column: "UserId", principalTable: "Users", principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdminNotes_Helpers_HelperId", table: "AdminNotes",
                column: "HelperId", principalTable: "Helpers", principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
