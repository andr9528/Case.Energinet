using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Case.Energinet.Persistence.Migrations
{
    public partial class UniqueIso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CachedRates_ISOCode",
                table: "CachedRates",
                column: "ISOCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CachedRates_ISOCode",
                table: "CachedRates");
        }
    }
}
