using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Data.Migrations
{
    /// <inheritdoc />
    public partial class GioHangTest1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GioHangId",
                table: "SanPham",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_GioHangId",
                table: "SanPham",
                column: "GioHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPham_GioHang_GioHangId",
                table: "SanPham",
                column: "GioHangId",
                principalTable: "GioHang",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SanPham_GioHang_GioHangId",
                table: "SanPham");

            migrationBuilder.DropIndex(
                name: "IX_SanPham_GioHangId",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "GioHangId",
                table: "SanPham");
        }
    }
}
