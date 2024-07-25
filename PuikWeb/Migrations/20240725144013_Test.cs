using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebUI.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b487e97-190d-4e93-8b03-2ebf60dd7523", "AQAAAAIAAYagAAAAECtqtFIsuSWVZltUg2F/6U4+UNGxFAJE1TBaLMzP55I+7KvxBL0vxYfCTLhrkQIEAA==", "dd4116d0-8d95-40d5-ad55-3c66b93adcb5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "19302f59-b591-48b5-a812-d7269f0760db", "AQAAAAIAAYagAAAAEKIOQ1E6BPWrYZZ8jyDq4Qxd55FJCEPL9GAq38syJDmV25fmgks8wzYsr0X01s+U/w==", "792dcf36-71f5-48ba-9bee-efd2a4c2ac2b" });
        }
    }
}
