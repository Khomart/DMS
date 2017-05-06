using ContosoUniversity.Models.SchoolViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.Entities
{
    public class Department : Entity
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "Department")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Chair")]
        public int? ProfessorID { get; set; }
        [Required]
        [ForeignKey("Faculty")]
        [Display(Name = "Faculty")]
        public int FacultyID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Faculty Faculty { get; set; }
        public Professor Administrator { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<UniversityProgram> Programs { get; set; }
        public virtual ICollection<Professor> Staff { get; set; }
    }
}