using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models.Entities;
namespace ContosoUniversity.Areas.Workflow.Models.AcademicYear
{
    public class AcademicYear
    {
        public int StartingDate { set; get; }
        public int EndDate { set; get; }
        public List<Semester> Semesters { set; get; }
        public string Year { get
            {
                return StartingDate + " - " + EndDate;
            }
        }
    }
}
