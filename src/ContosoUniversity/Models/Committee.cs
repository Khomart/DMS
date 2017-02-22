using ContosoUniversity.Models.UniversityFunctionalityModels;
using ContosoUniversity.UniversityFunctionalityModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public enum Level
    {
        Department, Faculty, University
    }
    public class Committee
    {
        [Key]
        public int CommitteeID { get; set; }
        [Display(Name = "Chair")]
        [Required]
        public int? ProfessorID { get; set; }
        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }
        [Display(Name = "Faculty")]
        public int? FacultyID { get; set; }
        [Required]
        public Level Level { get; set; }

        [ForeignKey("ProfessorID")]
        public Professor Chair { get; set; }
        [ForeignKey("DepartmentID")]
        public Department Department { get; set; }
        [ForeignKey("FacultyID")]
        public Faculty Faculty { get; set; }

        [StringLength(120, MinimumLength = 5)]
        [Required]
        [Display(Name = "Committee Title")]
        public string Title { get; set; }

        public virtual ICollection<CommitieMembership> CommitieMembers { get; set; }
        public virtual ICollection<Meetings> Meetings { get; set; }
        public virtual ICollection<FileBase> Files { set; get; }
    }
}
