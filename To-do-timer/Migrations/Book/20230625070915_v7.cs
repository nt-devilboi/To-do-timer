using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_do_timer.Migrations.Book
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookEvent");

            migrationBuilder.DropTable(
                name: "EventStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Events_BookId",
                table: "Events",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_StatusId",
                table: "Events",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Books_BookId",
                table: "Events",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Statuses_StatusId",
                table: "Events",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Books_BookId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Statuses_StatusId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_BookId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_StatusId",
                table: "Events");

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
    }
}
