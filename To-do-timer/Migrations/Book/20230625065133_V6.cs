using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_do_timer.Migrations.Book
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Events_EventId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Statuses_Events_EventId",
                table: "Statuses");

            migrationBuilder.DropIndex(
                name: "IX_Statuses_EventId",
                table: "Statuses");

            migrationBuilder.DropIndex(
                name: "IX_Books_EventId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BookEvent",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookEvent", x => new { x.BookId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_BookEvent_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookEvent_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventStatus",
                columns: table => new
                {
                    EventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStatus", x => new { x.EventsId, x.StatusId });
                    table.ForeignKey(
                        name: "FK_EventStatus_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventStatus_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookEvent_EventsId",
                table: "BookEvent",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_EventStatus_StatusId",
                table: "EventStatus",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookEvent");

            migrationBuilder.DropTable(
                name: "EventStatus");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Statuses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Books",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_EventId",
                table: "Statuses",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_EventId",
                table: "Books",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Events_EventId",
                table: "Books",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Statuses_Events_EventId",
                table: "Statuses",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
