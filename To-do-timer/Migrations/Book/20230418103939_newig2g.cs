using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_do_timer.Migrations.Book
{
    /// <inheritdoc />
    public partial class newig2g : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdStatus",
                table: "BookStatus",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "IdBook",
                table: "BookStatus",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdUser",
                table: "Books",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "BookStatus",
                newName: "IdStatus");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BookStatus",
                newName: "IdBook");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Books",
                newName: "IdUser");
        }
    }
}
