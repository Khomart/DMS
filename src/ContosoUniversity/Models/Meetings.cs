using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public class Meetings
    {
        [Display(Name = "Meeting Title")]
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Display(Name = "Meeting Date Suggestions")]
        public virtual List<DatesSuggestion> Suggestions { get; set; }
        [Display(Name = "Meeting Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        public DateTime OpenDate { get; set; }
        public bool FinalDate { get; set; }
        [StringLength(50)]
        [Display(Name = "Meeting Location")]
        public string Location { get; set; }

        public int MeetingID { get; set; }
        public int CommitteeID { get; set; }
        public Committee Committee { get; set; }

        //[StringLength(200)]
        //public string Description { get; set; }

        public virtual ICollection<MeetingComment> Comments { get; set; }
        public virtual ICollection<FileBase> Files { get; set; }
    }

    public class MeetingComment
    {
        [Key]
        public int CommentID { set; get; }

        public int MeetingID { set; get; }
        public int CommitteeID { set; get; }
        [Required]
        public bool Private { get; set; }

        [Required]
        [ForeignKey("Professor")]
        public int ProfessorID { set; get; }
        public int Professor { set; get; }
        [ForeignKey("MeetingID")]
        public Meetings Meeting { get; set; }

        [StringLength(350)]
        [Display(Name = "Comment")]
        public string Comment { set; get; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH-mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Written")]
        public DateTime DateStamp { set; get; }

        [Display(Name = "Professor Name")]
        public string ProfessorName { set; get; }
        public ICollection<FileBase> Files { set; get; }
    }
}
