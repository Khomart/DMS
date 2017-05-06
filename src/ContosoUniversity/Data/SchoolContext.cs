using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Models.Entities;

namespace ContosoUniversity.Data
{
    public class SchoolContext : IdentityDbContext< IdentityUser<int>, IdentityRole<int>, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<UniversityProgram> Programs { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Faculty> Facultys { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<CommitieMembership> CommitieMembership { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<TeachingRequest> TeachingRequests { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        //public DbSet<DepartmentEmploynment> Employments { get; set; }
        public DbSet<CoursePreference> RequestedCourses { get; set; }
        public DbSet<Meetings> Meetings { get; set; }
        public DbSet<DatesSuggestion> DatesSuggestion { get; set; }
        public DbSet<FileBase> FileBase { get; set; }
        public DbSet<MeetingComment> MeetComments { get; set; }
        public DbSet<Workload> Workloads { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser<int>>(i => {
                i.ToTable("Users");
                i.HasKey(x => x.Id);
            });
            modelBuilder.Entity<IdentityRole<int>>(i => {
                i.ToTable("Role");
                i.HasKey(x => x.Id);
            });
            modelBuilder.Entity<IdentityUserRole<int>>(i => {
                i.ToTable("UserRole");
                i.HasKey(x => new { x.RoleId, x.UserId });
            });
            modelBuilder.Entity<IdentityUserLogin<int>>(i => {
                i.ToTable("UserLogin");
                i.HasKey(x => new { x.ProviderKey, x.LoginProvider });
            });
            modelBuilder.Entity<IdentityRoleClaim<int>>(i => {
                i.ToTable("RoleClaims");
                i.HasKey(x => x.Id);
            });
            modelBuilder.Entity<IdentityUserClaim<int>>(i => {
                i.ToTable("UserClaims");
                i.HasKey(x => x.Id);
                
            });
            modelBuilder.Entity<IdentityUserToken<int>>(i => {
                i.ToTable("UserToken");
                i.HasKey(x => new {  x.UserId });
            });
            modelBuilder.Entity<Course>().ToTable("Course").HasKey(c => c.CourseID);
            modelBuilder.Entity<Course>().Property(c => c.CourseID).ValueGeneratedOnAdd();
            modelBuilder.Entity<UniversityProgram>().ToTable("Programs").HasKey(c => c.ProgramID);
            modelBuilder.Entity<UniversityProgram>().Property(c => c.ProgramID).ValueGeneratedOnAdd();
            modelBuilder.Entity<UniversityProgram>().HasOne(d => d.Department).WithMany(p => p.Programs).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Professor>().HasOne(i => i.Department).WithMany(i => i.Staff).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Department>().HasOne(a => a.Administrator).WithMany();
            modelBuilder.Entity<Department>().HasMany(d => d.Staff).WithOne(d => d.Department);
            //modelBuilder.Entity<Department>().HasOne(a => a.Administrator).WithMany().OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Faculty>().ToTable("Faculty");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment").HasKey(c => c.AssignmentID);
            modelBuilder.Entity<CourseAssignment>().Property(p => p.AssignmentID).ValueGeneratedOnAdd();
            //modelBuilder.Entity<DepartmentEmploynment>().ToTable("Employments").HasKey(c=> new { c.DepartmentID, c.ProfessorID });
            modelBuilder.Entity<Committee>().ToTable("Committee");
            modelBuilder.Entity<Committee>().HasOne(i => i.Chair).WithMany().OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<CommitieMembership>().ToTable("CommitieMembership").HasKey(c => new { c.CommitteeID, c.ProfessorID, c.DateOfEnrollment }) ;
            modelBuilder.Entity<Professor>().ToTable("Professors");
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<TeachingRequest>().ToTable("TeachingRequest").Property(p => p.RequestID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Semester>().ToTable("Semester");
            modelBuilder.Entity<CoursePreference>().ToTable("RequestedCourse").HasKey(c => new { c.CourseID, c.RequestID });
            modelBuilder.Entity<Meetings>().ToTable("Meeting").HasKey(c => new { c.MeetingID, c.CommitteeID }); ;
            modelBuilder.Entity<Meetings>().Property(p => p.MeetingID).ValueGeneratedOnAdd();
            modelBuilder.Entity<DatesSuggestion>().ToTable("DateSuggestion").HasKey(c => c.SuggestionID);
            modelBuilder.Entity<DatesSuggestion>().Property(p => p.SuggestionID).ValueGeneratedOnAdd();
            modelBuilder.Entity<DatesSuggestion>().HasOne(p => p.Meeting)
            .WithMany(c => c.Suggestions)
            .HasForeignKey(p => new { p.MeetingID, p.CommitteeID });
            modelBuilder.Entity<FileBase>().ToTable("FileBase").HasKey(c =>  c.FileBaseID );
            modelBuilder.Entity<MeetingComment>().ToTable("MeetingComment");
            modelBuilder.Entity<MeetingComment>().Property(p => p.CommentID).ValueGeneratedOnAdd();
            modelBuilder.Entity<MeetingComment>().HasOne(c => c.Meeting).WithMany(c => c.Comments).HasForeignKey(k => new { k.MeetingID, k.CommitteeID });
            modelBuilder.Entity<Workload>().ToTable("Workloads");

        }


    }
}