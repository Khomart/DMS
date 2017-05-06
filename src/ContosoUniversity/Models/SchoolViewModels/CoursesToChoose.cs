using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class CoursesToChoose
    {
        public int SemesterID { set; get; }
        public int ProfessorID { set; get; }
        public List<ChoosenCourse> Courses { get; set; }
    }
}
