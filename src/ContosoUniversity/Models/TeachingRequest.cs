using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.UniversityFunctionalityModels.Models;

namespace ContosoUniversity.Models
{
    public enum Desire
    {
        [Display(Name = "First Prefrence")]
        FirstPreference = 1,
        [Display(Name = "Second Prefrence")]
        SecondPreference = 2,
    }
    public class TeachingRequest
    {
        [Key]
        public int RequestID { get; set; }

        public int SemesterID { set; get; }
        public int ProfessorID { set; get; }
        public virtual ICollection<CoursePreference> ListOfCourses { set; get; }
        public virtual ICollection<Course> GivenCourses { set; get; }

        [ForeignKey("SemesterID")]
        public Semester SemesterForAssignment { set; get; }
        [ForeignKey("ProfessorID")]
        public Professor ProfessorEntity { set; get; }

        public string Annotation { set; get; } //Custom text field with request...
        [Display(Name ="Request Status")]
        public bool Approved { get; set; }
    }
}