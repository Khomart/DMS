using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContosoUniversity.Migrations
{
    public partial class meetings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateSuggestion_Users_ProfessorID",
                table: "DateSuggestion");

            migrationBuilder.DropForeignKey(
                name: "FK_DateSuggestion_Meeting_MeetingID1_MeetingCommitteeID",
                table: "DateSuggestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DateSuggestion",
                table: "DateSuggestion");

            migrationBuilder.DropIndex(
                name: "IX_DateSuggestion_ProfessorID",
                table: "DateSuggestion");

            migrationBuilder.DropIndex(
                name: "IX_DateSuggestion_MeetingID1_MeetingCommitteeID",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "ProfessorID",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "MeetingCommitteeID",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "MeetingID1",
                table: "DateSuggestion");

            migrationBuilder.AddColumn<int>(
                name: "DatesSuggestionSuggestionID",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuggestionID",
                table: "DateSuggestion",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "CommitteeID",
                table: "DateSuggestion",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Value",
                table: "DateSuggestion",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Users_DatesSuggestionSuggestionID",
                table: "Users",
                column: "DatesSuggestionSuggestionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DateSuggestion",
                table: "DateSuggestion",
                column: "SuggestionID");

            migrationBuilder.CreateIndex(
                name: "IX_DateSuggestion_MeetingID_CommitteeID",
                table: "DateSuggestion",
                columns: new[] { "MeetingID", "CommitteeID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DateSuggestion_Meeting_MeetingID_CommitteeID",
                table: "DateSuggestion",
                columns: new[] { "MeetingID", "CommitteeID" },
                principalTable: "Meeting",
                principalColumns: new[] { "MeetingID", "CommitteeID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_DateSuggestion_DatesSuggestionSuggestionID",
                table: "Users",
                column: "DatesSuggestionSuggestionID",
                principalTable: "DateSuggestion",
                principalColumn: "SuggestionID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DateSuggestion_Meeting_MeetingID_CommitteeID",
                table: "DateSuggestion");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_DateSuggestion_DatesSuggestionSuggestionID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DatesSuggestionSuggestionID",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DateSuggestion",
                table: "DateSuggestion");

            migrationBuilder.DropIndex(
                name: "IX_DateSuggestion_MeetingID_CommitteeID",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "DatesSuggestionSuggestionID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuggestionID",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "CommitteeID",
                table: "DateSuggestion");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "DateSuggestion");

            migrationBuilder.AddColumn<int>(
                name: "ProfessorID",
                table: "DateSuggestion",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "DateSuggestion",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MeetingCommitteeID",
                table: "DateSuggestion",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MeetingID1",
                table: "DateSuggestion",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DateSuggestion",
                table: "DateSuggestion",
                columns: new[] { "MeetingID", "ProfessorID" });

            migrationBuilder.CreateIndex(
                name: "IX_DateSuggestion_ProfessorID",
                table: "DateSuggestion",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_DateSuggestion_MeetingID1_MeetingCommitteeID",
                table: "DateSuggestion",
                columns: new[] { "MeetingID1", "MeetingCommitteeID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DateSuggestion_Users_ProfessorID",
                table: "DateSuggestion",
                column: "ProfessorID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DateSuggestion_Meeting_MeetingID1_MeetingCommitteeID",
                table: "DateSuggestion",
                columns: new[] { "MeetingID1", "MeetingCommitteeID" },
                principalTable: "Meeting",
                principalColumns: new[] { "MeetingID", "CommitteeID" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
