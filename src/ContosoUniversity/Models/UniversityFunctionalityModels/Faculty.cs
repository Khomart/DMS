using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.UniversityFunctionalityModels
{
    public class Faculty
    {
        [Key]
        public int FacultyID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int? ProfessorID { get; set; }
        [ForeignKey("ProfessorID")]
        public Professor Administrator { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
