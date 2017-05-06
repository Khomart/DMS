using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.Entities
{
    public enum Term
    {
        [Display(Name = "Summer First")]
        Summer_I = 1,
        [Display(Name = "Summer Second")]
        Summer_II = 2,
        //[Display(Name = "Summer Third")]
        //Summer_III = 3,
        [Display(Name = "Autumn")]
        Autumn = 3,
        [Display(Name = "Winter")]
        Winter = 4
    }
    public class Semester : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Semester")]
        public Term Season { set; get; }

        [Display(Name = "Academic Year")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StartYear { set; get; }
        public int EndYear
        {
            get
            {
                return StartYear + 1;
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Term Start")]
        public DateTime StartingDate { get; set; }
        [Display(Name = "Current")]
        public bool Current { set; get; }
        [Display(Name = "Open")]
        public bool Open { set; get; }
        public virtual ICollection<CourseAssignment> AssignedCourses { set; get; }
        //public virtual ICollection<Committee> CommitiesRunning { set; get; }
        public virtual ICollection<Enrollment> EnrollmentsInCourses { set; get; }
        public string Title
        {
            get
            {
                return Season + " of " + StartYear + " - " + (StartYear + 1);
            }
        }
    }
}
