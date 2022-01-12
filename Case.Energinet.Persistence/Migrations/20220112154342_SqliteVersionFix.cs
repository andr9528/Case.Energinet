using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Case.Energinet.Persistence.Migrations
{
    public partial class SqliteVersionFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Configs",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "CachedRates",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Version",
                table: "Configs",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Version",
                table: "CachedRates",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "BLOB",
                oldRowVersion: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
