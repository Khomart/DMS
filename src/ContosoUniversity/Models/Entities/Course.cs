using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.Entities
{
    public class Course : Entity
    {
        [Key]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 4)]
        [Required]
        [Display(Name = "Name")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Code")]
        [StringLength(20, MinimumLength = 3)]
        public string ShortTitle { get; set; }

        [Range(0, 7)]
        [Required]
        public int Credits { get; set; }

        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        public Department Department { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<CourseAssignment> Assignments { get; set; }
        public virtual ICollection<FileBase> Files { set; get; }

        [Display(Name = "Full Name")]
        public string FullTitile
        {
            get
            {
                if (Department != null)
                    return (Department.Name).Substring(0, 4) + CourseID;
                else return CourseID.ToString();
            }
        }
        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (Active == true)
                {
                    return "Active";
                }
                else
                {
                    return "Deactivated";
                }
            }
        }

        [Display(Name = "Active Course")]
        public bool Active { get; set; }

    }
}