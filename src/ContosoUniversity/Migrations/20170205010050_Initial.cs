using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContosoUniversity.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Semester",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Current = table.Column<bool>(nullable: false),
                    Open = table.Column<bool>(nullable: false),
                    Season = table.Column<int>(nullable: false),
                    StartYear = table.Column<int>(nullable: false),
                    StartingDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semester", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    HireDate = table.Column<DateTime>(nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    Approved = table.Column<bool>(nullable: true),
                    Program = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LoginProvider = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    IdentityRoleintId = table.Column<int>(name: "IdentityRole<int>Id", nullable: true),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Role_IdentityRole<int>Id",
                        column: x => x.IdentityRoleintId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfficeAssignment",
                columns: table => new
                {
                    ProfessorID = table.Column<int>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeAssignment", x => x.ProfessorID);
                    table.ForeignKey(
                        name: "FK_OfficeAssignment_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeachingRequest",
                columns: table => new
                {
                    RequestID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Annotation = table.Column<string>(nullable: true),
                    Approved = table.Column<bool>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false),
                    SemesterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingRequest", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_TeachingRequest_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeachingRequest_Semester_SemesterID",
                        column: x => x.SemesterID,
                        principalTable: "Semester",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faculty",
                columns: table => new
                {
                    FacultyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    ProfessorID = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculty", x => x.FacultyID);
                    table.ForeignKey(
                        name: "FK_Faculty_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    IdentityUserintId = table.Column<int>(name: "IdentityUser<int>Id", nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_IdentityUser<int>Id",
                        column: x => x.IdentityUserintId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    ProviderKey = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    IdentityUserintId = table.Column<int>(name: "IdentityUser<int>Id", nullable: true),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.ProviderKey, x.LoginProvider });
                    table.ForeignKey(
                        name: "FK_UserLogin_Users_IdentityUser<int>Id",
                        column: x => x.IdentityUserintId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IdentityRoleintId = table.Column<int>(name: "IdentityRole<int>Id", nullable: true),
                    IdentityUserintId = table.Column<int>(name: "IdentityUser<int>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_IdentityRole<int>Id",
                        column: x => x.IdentityRoleintId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRole_Users_IdentityUser<int>Id",
                        column: x => x.IdentityUserintId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FacultyID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    ProfessorID = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Department_Faculty_FacultyID",
                        column: x => x.FacultyID,
                        principalTable: "Faculty",
                        principalColumn: "FacultyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Department_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Committee",
                columns: table => new
                {
                    CommitteeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DepartmentID = table.Column<int>(nullable: true),
                    FacultyID = table.Column<int>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Committee", x => x.CommitteeID);
                    table.ForeignKey(
                        name: "FK_Committee_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committee_Faculty_FacultyID",
                        column: x => x.FacultyID,
                        principalTable: "Faculty",
                        principalColumn: "FacultyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Committee_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseID = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Credits = table.Column<int>(nullable: false),
                    DepartmentID = table.Column<int>(nullable: false),
                    TeachingRequestRequestID = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseID);
                    table.ForeignKey(
                        name: "FK_Course_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Course_TeachingRequest_TeachingRequestRequestID",
                        column: x => x.TeachingRequestRequestID,
                        principalTable: "TeachingRequest",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => new { x.DepartmentID, x.ProfessorID });
                    table.ForeignKey(
                        name: "FK_Employments_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employments_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitieMembership",
                columns: table => new
                {
                    CommitteeID = table.Column<int>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false),
                    DateOfEnrollment = table.Column<DateTime>(nullable: false),
                    Chair = table.Column<bool>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    EstimatedEndDate = table.Column<DateTime>(nullable: false),
                    FinishedWork = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitieMembership", x => new { x.CommitteeID, x.ProfessorID, x.DateOfEnrollment });
                    table.ForeignKey(
                        name: "FK_CommitieMembership_Committee_CommitteeID",
                        column: x => x.CommitteeID,
                        principalTable: "Committee",
                        principalColumn: "CommitteeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitieMembership_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meeting",
                columns: table => new
                {
                    MeetingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommitteeID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    FinalDate = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: true),
                    OpenDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meeting", x => new { x.MeetingID, x.CommitteeID });
                    table.ForeignKey(
                        name: "FK_Meeting_Committee_CommitteeID",
                        column: x => x.CommitteeID,
                        principalTable: "Committee",
                        principalColumn: "CommitteeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseAssignment",
                columns: table => new
                {
                    AssignmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignmentDate = table.Column<DateTime>(nullable: false),
                    CourseDescription = table.Column<string>(maxLength: 50, nullable: true),
                    CourseID = table.Column<int>(nullable: false),
                    CurrentlyTought = table.Column<bool>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false),
                    SemesterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAssignment", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_CourseAssignment_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAssignment_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAssignment_Semester_SemesterID",
                        column: x => x.SemesterID,
                        principalTable: "Semester",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestedCourse",
                columns: table => new
                {
                    CourseID = table.Column<int>(nullable: false),
                    RequestID = table.Column<int>(nullable: false),
                    Choice = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestedCourse", x => new { x.CourseID, x.RequestID });
                    table.ForeignKey(
                        name: "FK_RequestedCourse_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestedCourse_TeachingRequest_RequestID",
                        column: x => x.RequestID,
                        principalTable: "TeachingRequest",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    EnrollmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseID = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    SemesterID = table.Column<int>(nullable: true),
                    SmID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.EnrollmentID);
                    table.ForeignKey(
                        name: "FK_Enrollment_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollment_Semester_SemesterID",
                        column: x => x.SemesterID,
                        principalTable: "Semester",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollment_Users_SmID",
                        column: x => x.SmID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DateSuggestion",
                columns: table => new
                {
                    MeetingID = table.Column<int>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    MeetingCommitteeID = table.Column<int>(nullable: true),
                    MeetingID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateSuggestion", x => new { x.MeetingID, x.ProfessorID });
                    table.ForeignKey(
                        name: "FK_DateSuggestion_Users_ProfessorID",
                        column: x => x.ProfessorID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DateSuggestion_Meeting_MeetingID1_MeetingCommitteeID",
                        columns: x => new { x.MeetingID1, x.MeetingCommitteeID },
                        principalTable: "Meeting",
                        principalColumns: new[] { "MeetingID", "CommitteeID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingComment",
                columns: table => new
                {
                    CommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(maxLength: 250, nullable: true),
                    CommitteeID = table.Column<int>(nullable: false),
                    MeetingID = table.Column<int>(nullable: false),
                    Private = table.Column<bool>(nullable: false),
                    Professor = table.Column<int>(nullable: false),
                    ProfessorID = table.Column<int>(nullable: false),
                    ProfessorName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingComment", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_MeetingComment_Meeting_MeetingID_CommitteeID",
                        columns: x => new { x.MeetingID, x.CommitteeID },
                        principalTable: "Meeting",
                        principalColumns: new[] { "MeetingID", "CommitteeID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileBase",
                columns: table => new
                {
                    FileBaseID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Added = table.Column<DateTime>(nullable: false),
                    CommitteeID = table.Column<int>(nullable: true),
                    CourseID = table.Column<int>(nullable: true),
                    Location = table.Column<string>(nullable: false),
                    MeetingCommentCommentID = table.Column<int>(nullable: true),
                    MeetingsCommitteeID = table.Column<int>(nullable: true),
                    MeetingsMeetingID = table.Column<int>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    Owned = table.Column<int>(nullable: false),
                    OwnerID = table.Column<int>(nullable: false),
                    Size = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ViewTitle = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileBase", x => x.FileBaseID);
                    table.ForeignKey(
                        name: "FK_FileBase_Committee_CommitteeID",
                        column: x => x.CommitteeID,
                        principalTable: "Committee",
                        principalColumn: "CommitteeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileBase_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileBase_MeetingComment_MeetingCommentCommentID",
                        column: x => x.MeetingCommentCommentID,
                        principalTable: "MeetingComment",
                        principalColumn: "CommentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileBase_Meeting_MeetingsMeetingID_MeetingsCommitteeID",
                        columns: x => new { x.MeetingsMeetingID, x.MeetingsCommitteeID },
                        principalTable: "Meeting",
                        principalColumns: new[] { "MeetingID", "CommitteeID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommitieMembership_CommitteeID",
                table: "CommitieMembership",
                column: "CommitteeID");

            migrationBuilder.CreateIndex(
                name: "IX_CommitieMembership_ProfessorID",
                table: "CommitieMembership",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_Committee_DepartmentID",
                table: "Committee",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Committee_FacultyID",
                table: "Committee",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_Committee_ProfessorID",
                table: "Committee",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_DepartmentID",
                table: "Course",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TeachingRequestRequestID",
                table: "Course",
                column: "TeachingRequestRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignment_CourseID",
                table: "CourseAssignment",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignment_ProfessorID",
                table: "CourseAssignment",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssignment_SemesterID",
                table: "CourseAssignment",
                column: "SemesterID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestedCourse_CourseID",
                table: "RequestedCourse",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestedCourse_RequestID",
                table: "RequestedCourse",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_DateSuggestion_ProfessorID",
                table: "DateSuggestion",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_DateSuggestion_MeetingID1_MeetingCommitteeID",
                table: "DateSuggestion",
                columns: new[] { "MeetingID1", "MeetingCommitteeID" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_FacultyID",
                table: "Department",
                column: "FacultyID");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ProfessorID",
                table: "Department",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_CourseID",
                table: "Enrollment",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_SemesterID",
                table: "Enrollment",
                column: "SemesterID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_SmID",
                table: "Enrollment",
                column: "SmID");

            migrationBuilder.CreateIndex(
                name: "IX_FileBase_CommitteeID",
                table: "FileBase",
                column: "CommitteeID");

            migrationBuilder.CreateIndex(
                name: "IX_FileBase_CourseID",
                table: "FileBase",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_FileBase_MeetingCommentCommentID",
                table: "FileBase",
                column: "MeetingCommentCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_FileBase_MeetingsMeetingID_MeetingsCommitteeID",
                table: "FileBase",
                columns: new[] { "MeetingsMeetingID", "MeetingsCommitteeID" });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingComment_MeetingID_CommitteeID",
                table: "MeetingComment",
                columns: new[] { "MeetingID", "CommitteeID" });

            migrationBuilder.CreateIndex(
                name: "IX_Meeting_CommitteeID",
                table: "Meeting",
                column: "CommitteeID");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeAssignment_ProfessorID",
                table: "OfficeAssignment",
                column: "ProfessorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_DepartmentID",
                table: "Employments",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_ProfessorID",
                table: "Employments",
                column: "ProfessorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeachingRequest_ProfessorID",
                table: "TeachingRequest",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingRequest_SemesterID",
                table: "TeachingRequest",
                column: "SemesterID");

            migrationBuilder.CreateIndex(
                name: "IX_Faculty_ProfessorID",
                table: "Faculty",
                column: "ProfessorID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_IdentityRole<int>Id",
                table: "RoleClaims",
                column: "IdentityRole<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_IdentityUser<int>Id",
                table: "UserClaims",
                column: "IdentityUser<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_IdentityUser<int>Id",
                table: "UserLogin",
                column: "IdentityUser<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_IdentityRole<int>Id",
                table: "UserRole",
                column: "IdentityRole<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_IdentityUser<int>Id",
                table: "UserRole",
                column: "IdentityUser<int>Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommitieMembership");

            migrationBuilder.DropTable(
                name: "CourseAssignment");

            migrationBuilder.DropTable(
                name: "RequestedCourse");

            migrationBuilder.DropTable(
                name: "DateSuggestion");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "FileBase");

            migrationBuilder.DropTable(
                name: "OfficeAssignment");

            migrationBuilder.DropTable(
                name: "Employments");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "MeetingComment");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "TeachingRequest");

            migrationBuilder.DropTable(
                name: "Meeting");

            migrationBuilder.DropTable(
                name: "Semester");

            migrationBuilder.DropTable(
                name: "Committee");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Faculty");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
