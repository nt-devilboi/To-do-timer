using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace To_do_timer.Migrations.Book
{
    /// <inheritdoc />
    public partial class V5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
