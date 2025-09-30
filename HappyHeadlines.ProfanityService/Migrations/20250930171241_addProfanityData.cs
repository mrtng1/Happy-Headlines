using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HappyHeadlines.ProfanityService.Migrations
{
    /// <inheritdoc />
    public partial class addProfanityData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProfanityWords",
                columns: new[] { "Id", "Word" },
                values: new object[,]
                {
                    { 1, "badword1" },
                    { 2, "badword2" },
                    { 3, "badword3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProfanityWords",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProfanityWords",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProfanityWords",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
