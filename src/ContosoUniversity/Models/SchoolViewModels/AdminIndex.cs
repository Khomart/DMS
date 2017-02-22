using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class AdminIndex
    {
        public IEnumerable<Student> NewStudents { get; set; }
        public IEnumerable<TeachingRequest> Requests { get; set; }
        public IEnumerable<Committee> Commities { get; set; }
        //public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}
