using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.Entities
{
    public class UniversityProgram : Entity
    {
        [Key]
        public int ProgramID { get; set; }

        [StringLength(50, MinimumLength = 5)]
        [Required]
        [Display(Name = "Name")]
        public string Title { get; set; }

        [Display(Name = "Short Name")]
        [StringLength(20, MinimumLength = 3)]
        public string Short { get; set; }
        [Required]
        [Display(Name ="Department")]
        public int DepartmentID { get; set; }

        public Department Department { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        

    }
}
