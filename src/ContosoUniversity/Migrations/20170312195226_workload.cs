using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoUniversity.Migrations
{
    public partial class workload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workloads",
                columns: table => new
                {
                    ProfessorID = table.Column<int>(nullable: false),
                    AcBooksAndMono = table.Column<int>(nullable: false),
                    AccJourArt = table.Column<int>(nullable: false),
                    ChaptInBooks = table.Column<int>(nullable: false),
                    CompButUnpubPapers = table.Column<int>(nullable: false),
                    DepCommittees = table.Column<int>(nullable: false),
                    DiscAndTrChairAndOther = table.Column<int>(nullable: false),
                    Duties = table.Column<int>(nullable: false),
                    EditedBooks = table.Column<int>(nullable: false),
                    ExtResGrantApplications = table.Column<int>(nullable: false),
                    ExtResGrantReceived = table.Column<int>(nullable: false),
                    FacCommittees = table.Column<int>(nullable: false),
                    IntAndNatConferences = table.Column<int>(nullable: false),
                    IntResGrantReceived = table.Column<int>(nullable: false),
                    MBA = table.Column<int>(nullable: false),
                    MSc = table.Column<int>(nullable: false),
                    NonRefJourArtic = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(maxLength: 200, nullable: true),
                    OtherPresentations = table.Column<int>(nullable: false),
                    OtherPublications = table.Column<int>(nullable: false),
                    PHDPhase2 = table.Column<int>(nullable: false),
                    PHDPhase3 = table.Column<int>(nullable: false),
                    ProfAssociatons = table.Column<int>(nullable: false),
                    ProjInProgress = table.Column<int>(nullable: false),
                    PubBooksRevs = table.Column<int>(nullable: false),
                    RefConPro = table.Column<int>(nullable: false),
                    RefJourArt = table.Column<int>(nullable: false),
                    RegConferences = table.Column<int>(nullable: false),
                    SemesterID = table.Column<int>(nullable: false),
                    SympWorkshops = table.Column<int>(nullable: false),
                    Textbooks = table.Column<int>(nullable: false),
                    UnivCommittees = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workloads", x => x.ProfessorID);
                    table.ForeignKey(
                        name: "FK_Workloads_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workloads_Semester_SemesterID",
                        column: x => x.SemesterID,
                        principalTable: "Semester",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workloads_ProfessorID",
                table: "Workloads",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_Workloads_SemesterID",
                table: "Workloads",
                column: "SemesterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Workloads");
        }
    }
}
