using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.Entities
{
    public enum Level
    {
        Department, Faculty, University
    }
    public class Committee : Entity
    {
        [Key]
        public int CommitteeID { get; set; }
        [Display(Name = "Chair")]
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
        [StringLength(300, MinimumLength = 5)]
        [Required]
        [Display(Name = "Description")]
        public string Commentary { set; get; }

        public virtual ICollection<CommitieMembership> CommitieMembers { get; set; }
        public virtual ICollection<Meetings> Meetings { get; set; }
        public virtual ICollection<FileBase> Files { set; get; }
    }
}
