using ContosoUniversity.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Workload
    {
        [Key]
        public int WorkloadID { set; get; }
        [ForeignKey("Professor")]
        [Required]
        public int ProfessorID { get; set; }
        //[ForeignKey("Semester")]
        //public int SemesterID { get; set; }
        public virtual Professor Professor { get; set; }
        //public virtual Semester Semester { get; set; }
        public bool Finished { set; get; }
        public bool Reviewed { set;get; }


        [Display(Name ="Year")]
        [Required]
        public int Year { set; get; }
        //Publications
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Refereed Journal Articles", GroupName = "Publications")]
        public int RefJourArt { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Accepted Journal Articles", GroupName = "Publications")]
        public int AccJourArt { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Refereed Conference Proceedings", GroupName = "Publications")]
        public int RefConPro { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Academic Books and Monographs", GroupName ="Publications")]
        public int AcBooksAndMono { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Edited Books", GroupName = "Publications")]
        public int EditedBooks { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Chapters in Books", GroupName = "Publications")]
        public int ChaptInBooks { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Published Book Reviews", GroupName = "Publications")]
        public int PubBooksRevs { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Textbooks", GroupName = "Publications")]
        public int Textbooks { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Non-refereed Journal Articles", GroupName = "Publications")]
        public int NonRefJourArtic { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Other Publications", GroupName = "Publications")]
        public int OtherPublications { set; get; }

        //Research Grants
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Number of External Research Grant Applications", GroupName = "Research Grants")]
        public int ExtResGrantApplications { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Amount of External Research Grants Received", GroupName = "Research Grants")]
        public int ExtResGrantReceived { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Amount of Internal Research Grants Received", GroupName = "Research Grants")]
        public int IntResGrantReceived { set; get; }

        //Conference Presentation
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Int'l and National Conferences", GroupName = "Conference Presentation")]
        public int IntAndNatConferences { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Regional Conferences", GroupName = "Conference Presentation")]
        public int RegConferences { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Symposia Workshops", GroupName = "Conference Presentation")]
        public int SympWorkshops { set; get; }


        //Research Supervisions
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Discussant, Track Chair, etc", GroupName = "Research Supervisions")]
        public int DiscAndTrChairAndOther { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Doctor of Philosophy Phase III", GroupName = "Research Supervisions")]
        public int PHDPhase3 { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Doctor of Philosophy Phase II", GroupName = "Research Supervisions")]
        public int PHDPhase2 { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Master of Business Administration", GroupName = "Research Supervisions")]
        public int MBA { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Master of Business Science", GroupName = "Research Supervisions")]
        public int MSc { set; get; }

        //Other Scholarly Activities
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Other Presentations", GroupName = "Other Scholarly Activities")]
        public int OtherPresentations { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Completed but Unpublished Papers", GroupName = "Other Scholarly Activities")]
        public int CompButUnpubPapers { set; get; }

        //Service to the University and Community
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Projects in Progress", GroupName = "Research Supervisions")]
        public int ProjInProgress { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "University Committees", GroupName = "Research Supervisions")]
        public int UnivCommittees { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Faculty Committees", GroupName = "Research Supervisions")]
        public int FacCommittees { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Department Committees", GroupName = "Research Supervisions")]
        public int DepCommittees { set; get; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Value has to be a number")]
        [Display(Name = "Professional Associations", GroupName = "Research Supervisions")]
        public int ProfAssociatons { set; get; }

        //Duties
        [Display(Name = "Duties")]
        public int Duties { set; get; }

        [Display(Name = "Notes")]
        [StringLength(200)]
        public string Notes { set; get; }
    }
}
