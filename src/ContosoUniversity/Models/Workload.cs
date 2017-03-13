using ContosoUniversity.UniversityFunctionalityModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public class Workload
    {
        [ForeignKey("Professor")]
        [Key]
        public int ProfessorID { get; set; }
        [ForeignKey("Semester")]
        public int SemesterID { get; set; }
        public virtual Professor Professor { get; set; }
        public virtual Semester Semester { get; set; }


        [Display(Name ="Year")]
        [Required]
        public int Year { set; get; }
        //Publications
        [Display(Name = "Refereed Journal Articles", GroupName = "Publications")]
        public int RefJourArt { get; set; }
        [Display(Name = "Accepted Journal Articles", GroupName = "Publications")]
        public int AccJourArt { get; set; }
        [Display(Name = "Refereed Conference Proceedings", GroupName = "Publications")]
        public int RefConPro { set; get; }
        [Display(Name = "Academic Books and Monographs", GroupName ="Publications")]
        public int AcBooksAndMono { get; set; }
        [Display(Name = "Edited Books", GroupName = "Publications")]
        public int EditedBooks { get; set; }
        [Display(Name = "Chapters in Books", GroupName = "Publications")]
        public int ChaptInBooks { set; get; }
        [Display(Name = "Published Book Reviews", GroupName = "Publications")]
        public int PubBooksRevs { get; set; }
        [Display(Name = "Textbooks", GroupName = "Publications")]
        public int Textbooks { get; set; }
        [Display(Name = "Non-refereed Journal Articles", GroupName = "Publications")]
        public int NonRefJourArtic { set; get; }
        [Display(Name = "Other Publications", GroupName = "Publications")]
        public int OtherPublications { set; get; }

        //Research Grants
        [Display(Name = "Number of External Research Grant Applications", GroupName = "Research Grants")]
        public int ExtResGrantApplications { get; set; }
        [Display(Name = "Amount of External Research Grants Received", GroupName = "Research Grants")]
        public int ExtResGrantReceived { set; get; }
        [Display(Name = "Amount of Internal Research Grants Received", GroupName = "Research Grants")]
        public int IntResGrantReceived { set; get; }

        //Conference Presentation
        [Display(Name = "Int'l and National Conferences", GroupName = "Conference Presentation")]
        public int IntAndNatConferences { get; set; }
        [Display(Name = "Regional Conferences", GroupName = "Conference Presentation")]
        public int RegConferences { set; get; }
        [Display(Name = "Symposia Workshops", GroupName = "Conference Presentation")]
        public int SympWorkshops { set; get; }


        //Research Supervisions
        [Display(Name = "Discussant, Track Chair, etc", GroupName = "Research Supervisions")]
        public int DiscAndTrChairAndOther { get; set; }
        [Display(Name = "Ph.D. Phase III", GroupName = "Research Supervisions")]
        public int PHDPhase3 { set; get; }
        [Display(Name = "Ph.D. Phase II", GroupName = "Research Supervisions")]
        public int PHDPhase2 { set; get; }
        [Display(Name = "MBA", GroupName = "Research Supervisions")]
        public int MBA { set; get; }
        [Display(Name = "M.Sc.", GroupName = "Research Supervisions")]
        public int MSc { set; get; }

        //Other Scholarly Activities
        [Display(Name = "Other Presentations", GroupName = "Other Scholarly Activities")]
        public int OtherPresentations { get; set; }
        [Display(Name = "Completed but Unpublished Papers", GroupName = "Other Scholarly Activities")]
        public int CompButUnpubPapers { set; get; }

        //Service to the University and Community
        [Display(Name = "Projects in Progress", GroupName = "Research Supervisions")]
        public int ProjInProgress { set; get; }
        [Display(Name = "University Committees", GroupName = "Research Supervisions")]
        public int UnivCommittees { set; get; }
        [Display(Name = "Faculty Committees", GroupName = "Research Supervisions")]
        public int FacCommittees { set; get; }
        [Display(Name = "Department Committees", GroupName = "Research Supervisions")]
        public int DepCommittees { set; get; }
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
