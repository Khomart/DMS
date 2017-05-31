using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ContosoUniversity.Data;

namespace ContosoUniversity.Migrations
{
    [DbContext(typeof(SchoolContext))]
    [Migration("20170511203028_newDrop")]
    partial class newDrop
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContosoUniversity.Models.CommitieMembership", b =>
                {
                    b.Property<int>("CommitteeID");

                    b.Property<int>("ProfessorID");

                    b.Property<DateTime>("DateOfEnrollment");

                    b.Property<bool>("Chair");

                    b.Property<DateTime>("EndDate");

                    b.Property<DateTime>("EstimatedEndDate");

                    b.Property<bool>("FinishedWork");

                    b.HasKey("CommitteeID", "ProfessorID", "DateOfEnrollment");

                    b.HasIndex("CommitteeID");

                    b.HasIndex("ProfessorID");

                    b.ToTable("CommitieMembership");
                });

            modelBuilder.Entity("ContosoUniversity.Models.CourseAssignment", b =>
                {
                    b.Property<int>("AssignmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AssignmentDate");

                    b.Property<string>("CourseDescription")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("CourseID");

                    b.Property<bool>("CurrentlyTought");

                    b.Property<int>("ProfessorID");

                    b.Property<int>("SemesterID");

                    b.HasKey("AssignmentID");

                    b.HasIndex("CourseID");

                    b.HasIndex("ProfessorID");

                    b.HasIndex("SemesterID");

                    b.ToTable("CourseAssignment");
                });

            modelBuilder.Entity("ContosoUniversity.Models.CoursePreference", b =>
                {
                    b.Property<int>("CourseID");

                    b.Property<int>("RequestID");

                    b.Property<int>("Choice");

                    b.HasKey("CourseID", "RequestID");

                    b.HasIndex("CourseID");

                    b.HasIndex("RequestID");

                    b.ToTable("RequestedCourse");
                });

            modelBuilder.Entity("ContosoUniversity.Models.DatesSuggestion", b =>
                {
                    b.Property<int>("SuggestionID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommitteeID");

                    b.Property<int>("MeetingID");

                    b.Property<DateTime>("Value");

                    b.HasKey("SuggestionID");

                    b.HasIndex("MeetingID", "CommitteeID");

                    b.ToTable("DateSuggestion");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Enrollment", b =>
                {
                    b.Property<int>("EnrollmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CourseID");

                    b.Property<int?>("Grade");

                    b.Property<string>("Notes");

                    b.Property<int?>("SemesterID");

                    b.Property<int>("SmID");

                    b.HasKey("EnrollmentID");

                    b.HasIndex("CourseID");

                    b.HasIndex("SemesterID");

                    b.HasIndex("SmID");

                    b.ToTable("Enrollment");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Committee", b =>
                {
                    b.Property<int>("CommitteeID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archived");

                    b.Property<int?>("DepartmentID");

                    b.Property<int?>("FacultyID");

                    b.Property<int>("Level");

                    b.Property<int?>("ProfessorID");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 120);

                    b.HasKey("CommitteeID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("FacultyID");

                    b.HasIndex("ProfessorID");

                    b.ToTable("Committee");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Course", b =>
                {
                    b.Property<int>("CourseID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<bool>("Archived");

                    b.Property<int>("Credits");

                    b.Property<int>("DepartmentID");

                    b.Property<string>("ShortTitle")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.Property<int?>("TeachingRequestRequestID");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("CourseID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("TeachingRequestRequestID");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archived");

                    b.Property<int>("FacultyID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("ProfessorID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("DepartmentID");

                    b.HasIndex("FacultyID");

                    b.HasIndex("ProfessorID");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Faculty", b =>
                {
                    b.Property<int>("FacultyID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archived");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int?>("ProfessorID");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("FacultyID");

                    b.HasIndex("ProfessorID");

                    b.ToTable("Faculty");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Semester", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archived");

                    b.Property<bool>("Current");

                    b.Property<bool>("Open");

                    b.Property<int>("Season");

                    b.Property<int>("StartYear");

                    b.Property<DateTime>("StartingDate");

                    b.HasKey("ID");

                    b.ToTable("Semester");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.UniversityProgram", b =>
                {
                    b.Property<int>("ProgramID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archived");

                    b.Property<int>("DepartmentID");

                    b.Property<string>("Short")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("ProgramID");

                    b.HasIndex("DepartmentID");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("ContosoUniversity.Models.FileBase", b =>
                {
                    b.Property<int>("FileBaseID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Added");

                    b.Property<int?>("CommitteeID");

                    b.Property<int?>("CourseID");

                    b.Property<string>("Location")
                        .IsRequired();

                    b.Property<int?>("MeetingCommentCommentID");

                    b.Property<int?>("MeetingsCommitteeID");

                    b.Property<int?>("MeetingsMeetingID");

                    b.Property<DateTime>("Modified");

                    b.Property<int>("Owned");

                    b.Property<int>("OwnerID");

                    b.Property<int>("Size");

                    b.Property<int>("Type");

                    b.Property<string>("ViewTitle")
                        .IsRequired();

                    b.HasKey("FileBaseID");

                    b.HasIndex("CommitteeID");

                    b.HasIndex("CourseID");

                    b.HasIndex("MeetingCommentCommentID");

                    b.HasIndex("MeetingsMeetingID", "MeetingsCommitteeID");

                    b.ToTable("FileBase");
                });

            modelBuilder.Entity("ContosoUniversity.Models.MeetingComment", b =>
                {
                    b.Property<int>("CommentID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment")
                        .HasAnnotation("MaxLength", 350);

                    b.Property<string>("CommentTitle")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("CommitteeID");

                    b.Property<DateTime>("DateStamp");

                    b.Property<int>("MeetingID");

                    b.Property<bool>("Private");

                    b.Property<int?>("ProfessorID");

                    b.Property<string>("ProfessorName");

                    b.Property<bool>("adminComment");

                    b.HasKey("CommentID");

                    b.HasIndex("ProfessorID");

                    b.HasIndex("MeetingID", "CommitteeID");

                    b.ToTable("MeetingComment");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Meetings", b =>
                {
                    b.Property<int>("MeetingID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommitteeID");

                    b.Property<DateTime>("Date");

                    b.Property<bool>("FinalDate");

                    b.Property<string>("Location")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("OpenDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("MeetingID", "CommitteeID");

                    b.HasIndex("CommitteeID");

                    b.ToTable("Meeting");
                });

            modelBuilder.Entity("ContosoUniversity.Models.OfficeAssignment", b =>
                {
                    b.Property<int>("ProfessorID");

                    b.Property<string>("Location")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("ProfessorID");

                    b.HasIndex("ProfessorID")
                        .IsUnique();

                    b.ToTable("OfficeAssignment");
                });

            modelBuilder.Entity("ContosoUniversity.Models.TeachingRequest", b =>
                {
                    b.Property<int>("RequestID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Annotation");

                    b.Property<bool>("Approved");

                    b.Property<int>("ProfessorID");

                    b.Property<int>("SemesterID");

                    b.HasKey("RequestID");

                    b.HasIndex("ProfessorID");

                    b.HasIndex("SemesterID");

                    b.ToTable("TeachingRequest");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Workload", b =>
                {
                    b.Property<int>("WorkloadID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AcBooksAndMono");

                    b.Property<int>("AccJourArt");

                    b.Property<int>("ChaptInBooks");

                    b.Property<int>("CompButUnpubPapers");

                    b.Property<int>("DepCommittees");

                    b.Property<int>("DiscAndTrChairAndOther");

                    b.Property<int>("Duties");

                    b.Property<int>("EditedBooks");

                    b.Property<int>("ExtResGrantApplications");

                    b.Property<int>("ExtResGrantReceived");

                    b.Property<int>("FacCommittees");

                    b.Property<bool>("Finished");

                    b.Property<int>("IntAndNatConferences");

                    b.Property<int>("IntResGrantReceived");

                    b.Property<int>("MBA");

                    b.Property<int>("MSc");

                    b.Property<int>("NonRefJourArtic");

                    b.Property<string>("Notes")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<int>("OtherPresentations");

                    b.Property<int>("OtherPublications");

                    b.Property<int>("PHDPhase2");

                    b.Property<int>("PHDPhase3");

                    b.Property<int>("ProfAssociatons");

                    b.Property<int>("ProfessorID");

                    b.Property<int>("ProjInProgress");

                    b.Property<int>("PubBooksRevs");

                    b.Property<int>("RefConPro");

                    b.Property<int>("RefJourArt");

                    b.Property<int>("RegConferences");

                    b.Property<bool>("Reviewed");

                    b.Property<int>("SympWorkshops");

                    b.Property<int>("Textbooks");

                    b.Property<int>("UnivCommittees");

                    b.Property<int>("Year");

                    b.HasKey("WorkloadID");

                    b.HasIndex("ProfessorID");

                    b.ToTable("Workloads");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedName");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int?>("IdentityRole<int>Id");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("IdentityRole<int>Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser<int>");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int?>("IdentityUser<int>Id");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("IdentityUser<int>Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("ProviderKey");

                    b.Property<string>("LoginProvider");

                    b.Property<int?>("IdentityUser<int>Id");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("ProviderKey", "LoginProvider");

                    b.HasIndex("IdentityUser<int>Id");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<int>("UserId");

                    b.Property<int?>("IdentityRole<int>Id");

                    b.Property<int?>("IdentityUser<int>Id");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("IdentityRole<int>Id");

                    b.HasIndex("IdentityUser<int>Id");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Admin", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>");


                    b.ToTable("Admins");

                    b.HasDiscriminator().HasValue("Admin");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Professor", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>");

                    b.Property<bool>("Archived");

                    b.Property<int?>("DatesSuggestionSuggestionID");

                    b.Property<int>("DepartmentID");

                    b.Property<string>("FirstMidName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("HireDate");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasIndex("DatesSuggestionSuggestionID");

                    b.HasIndex("DepartmentID");

                    b.ToTable("Professors");

                    b.HasDiscriminator().HasValue("Professor");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Student", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>");

                    b.Property<bool>("Approved");

                    b.Property<bool>("Archived");

                    b.Property<string>("FirstMidName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("ProgramID");

                    b.HasIndex("ProgramID");

                    b.ToTable("Student");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("ContosoUniversity.Models.CommitieMembership", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Committee", "Committee")
                        .WithMany("CommitieMembers")
                        .HasForeignKey("CommitteeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.Professor", "Professor")
                        .WithMany("Commities")
                        .HasForeignKey("ProfessorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.CourseAssignment", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Course", "Course")
                        .WithMany("Assignments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.Professor", "Professor")
                        .WithMany("Courses")
                        .HasForeignKey("ProfessorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.Entities.Semester", "Semester")
                        .WithMany("AssignedCourses")
                        .HasForeignKey("SemesterID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.CoursePreference", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.TeachingRequest", "Request")
                        .WithMany("ListOfCourses")
                        .HasForeignKey("RequestID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.DatesSuggestion", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Meetings", "Meeting")
                        .WithMany("Suggestions")
                        .HasForeignKey("MeetingID", "CommitteeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.Enrollment", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Course", "Course")
                        .WithMany("Enrollments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.Entities.Semester")
                        .WithMany("EnrollmentsInCourses")
                        .HasForeignKey("SemesterID");

                    b.HasOne("ContosoUniversity.Models.Student", "Student")
                        .WithMany("Enrollments")
                        .HasForeignKey("SmID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Committee", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentID");

                    b.HasOne("ContosoUniversity.Models.Entities.Faculty", "Faculty")
                        .WithMany()
                        .HasForeignKey("FacultyID");

                    b.HasOne("ContosoUniversity.Models.Professor", "Chair")
                        .WithMany()
                        .HasForeignKey("ProfessorID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Course", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Department", "Department")
                        .WithMany("Courses")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.TeachingRequest")
                        .WithMany("GivenCourses")
                        .HasForeignKey("TeachingRequestRequestID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Department", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Faculty", "Faculty")
                        .WithMany("Departments")
                        .HasForeignKey("FacultyID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.Professor", "Administrator")
                        .WithMany()
                        .HasForeignKey("ProfessorID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.Faculty", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Professor", "Administrator")
                        .WithMany()
                        .HasForeignKey("ProfessorID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Entities.UniversityProgram", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Department", "Department")
                        .WithMany("Programs")
                        .HasForeignKey("DepartmentID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.FileBase", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Committee")
                        .WithMany("Files")
                        .HasForeignKey("CommitteeID");

                    b.HasOne("ContosoUniversity.Models.Entities.Course")
                        .WithMany("Files")
                        .HasForeignKey("CourseID");

                    b.HasOne("ContosoUniversity.Models.MeetingComment")
                        .WithMany("Files")
                        .HasForeignKey("MeetingCommentCommentID");

                    b.HasOne("ContosoUniversity.Models.Meetings")
                        .WithMany("Files")
                        .HasForeignKey("MeetingsMeetingID", "MeetingsCommitteeID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.MeetingComment", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Professor", "Professor")
                        .WithMany()
                        .HasForeignKey("ProfessorID");

                    b.HasOne("ContosoUniversity.Models.Meetings", "Meeting")
                        .WithMany("Comments")
                        .HasForeignKey("MeetingID", "CommitteeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.Meetings", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.Committee", "Committee")
                        .WithMany("Meetings")
                        .HasForeignKey("CommitteeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.OfficeAssignment", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Professor", "Professor")
                        .WithOne("OfficeAssignment")
                        .HasForeignKey("ContosoUniversity.Models.OfficeAssignment", "ProfessorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.TeachingRequest", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Professor", "ProfessorEntity")
                        .WithMany("TeachingRequests")
                        .HasForeignKey("ProfessorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Models.Entities.Semester", "SemesterForAssignment")
                        .WithMany()
                        .HasForeignKey("SemesterID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Models.Workload", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Professor", "Professor")
                        .WithMany("Workloads")
                        .HasForeignKey("ProfessorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>")
                        .WithMany("Claims")
                        .HasForeignKey("IdentityRole<int>Id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>")
                        .WithMany("Claims")
                        .HasForeignKey("IdentityUser<int>Id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>")
                        .WithMany("Logins")
                        .HasForeignKey("IdentityUser<int>Id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole<int>")
                        .WithMany("Users")
                        .HasForeignKey("IdentityRole<int>Id");

                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser<int>")
                        .WithMany("Roles")
                        .HasForeignKey("IdentityUser<int>Id");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Professor", b =>
                {
                    b.HasOne("ContosoUniversity.Models.DatesSuggestion")
                        .WithMany("Checkers")
                        .HasForeignKey("DatesSuggestionSuggestionID");

                    b.HasOne("ContosoUniversity.Models.Entities.Department", "Department")
                        .WithMany("Staff")
                        .HasForeignKey("DepartmentID");
                });

            modelBuilder.Entity("ContosoUniversity.Models.Student", b =>
                {
                    b.HasOne("ContosoUniversity.Models.Entities.UniversityProgram", "Program")
                        .WithMany("Students")
                        .HasForeignKey("ProgramID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
