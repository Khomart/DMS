using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.Models.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ContosoUniversity.Models
{
    public class Professor : IdentityUser<int>
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }
        public virtual List<CourseAssignment> Courses { get; set; }
        public OfficeAssignment OfficeAssignment { get; set; }

        public virtual ICollection<CommitieMembership> Commities { get; set; }
        public virtual ICollection<TeachingRequest> TeachingRequests { get; set; }

        [Display(Name = "Department")]
        public int DepartmentID { get; set; }
        [ForeignKey("DepartmentID")]
        public Department Department { get; set; }
        public virtual ICollection<Workload> Workloads { set; get; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + " " + FirstMidName;
            }
        }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public bool Archived { set; get; }

        public static explicit operator Professor(EntityEntry v)
        {
            throw new NotImplementedException();
        }
    }
}
