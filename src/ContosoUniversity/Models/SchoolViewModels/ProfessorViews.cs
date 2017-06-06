using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class ProfessorViews
    {
        public Professor Professors { get; set; }
        public IEnumerable<CourseAssignment> CoursesAssignments { get; set; }
        public IEnumerable<CommitieMembership> Membership { get; set; }
        public IEnumerable<TeachingRequest> Requests { get; set;  }
        
        public IEnumerable<Meetings> Meetings { set; get; }
    }
}
