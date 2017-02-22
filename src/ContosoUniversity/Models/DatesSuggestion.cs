using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class DatesSuggestion
    {
        //public int MeetingID { get; set; }
        //public int ProfessorID { get; set; }
        //public Meetings Meeting { get; set; }
        //public Professor Professor { get; set; }


        //[Display(Name = "Date Suggestion")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yy H:mm}", ApplyFormatInEditMode = true)]
        //public DateTime Date { get; set; }
            public int SuggestionID { set; get; }
            [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yy HH:mm}")]
            [Required]
            public DateTime Value { get; set; }

            //list of members checked datetimeField
            public virtual ICollection<Professor> Checkers { set; get; }

            public virtual Meetings Meeting { set; get; }
            public int MeetingID { set; get; }
            public int CommitteeID { get; set; }

    }
}
