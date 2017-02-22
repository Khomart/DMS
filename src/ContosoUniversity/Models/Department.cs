using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Models.UniversityFunctionalityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        //[DataType(DataType.Currency)]
        //[Column(TypeName = "money")]
        //public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int? ProfessorID { get; set; }
        [Required]
        [ForeignKey("Faculty")]
        public int FacultyID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Faculty Faculty { get; set; }
        public Professor Administrator { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<DepartmentEmploynment> Staff { get; set; }
    }
}