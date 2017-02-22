using ContosoUniversity.UniversityFunctionalityModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public class CommitieMembership
    {
        public int ProfessorID { get; set; }
        public int CommitteeID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Join Date")]
        public DateTime DateOfEnrollment { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expected Leaving")]
        public DateTime EstimatedEndDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Actual Left")]
        public DateTime EndDate { get; set; }

        //public bool Chair { get; set; }
        [ForeignKey("ProfessorID")]
        public Professor Professor { get; set; }
        [ForeignKey("CommitteeID")]
        public Committee Committee { get; set; }
                                                                
        [Display(Name = "Chair")]
        [Required]
        public bool Chair { get; set; }
        public bool FinishedWork { get; set; }
        public string Status
        {
            get
            {
                if (Chair == true)
                    return "Chair";
                else
                    return "Member";
            }
        }
        public string IsActive
        {
            get
            {
                if (FinishedWork == true)
                    return "Past";
                else
                    return "Present";
            }
        }
    }
}
