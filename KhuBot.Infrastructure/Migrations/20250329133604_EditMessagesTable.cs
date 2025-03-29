using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhuBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromBot",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "ChatMessages",
                newName: "ResponseTimeStamp");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ChatMessages",
                newName: "Response");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "MessageTimeStamp",
                table: "ChatMessages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "MessageTimeStamp",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "ResponseTimeStamp",
                table: "ChatMessages",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "ChatMessages",
                newName: "Content");

            migrationBuilder.AddColumn<bool>(
                name: "IsFromBot",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
