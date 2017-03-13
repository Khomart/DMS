using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//courses assigned to Student
namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int SmID { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }
        public string Notes { get; set; }
        [ForeignKey("CourseID")]
        public Course Course { get; set; }
        [ForeignKey("SmID")]
        public Student Student { get; set; }

        //[ForeignKey("Semester")]
        //public int SemesterID { get; set; }
        //public Semester Semester { set; get; }
    }
}