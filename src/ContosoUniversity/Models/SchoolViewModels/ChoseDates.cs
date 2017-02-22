using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class ChoseDates
    {
        public int ProfessorID { set; get; }
        public int MeetingID { set; get; }
        public class DateChoice
        {
            public DatesSuggestion date { set; get; }
            public bool choice { set; get; }
        }
        public List<DateChoice> Dates { set; get; }
    }
}
