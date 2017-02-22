using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class RequestReview
    {
        public TeachingRequest Request { get; set; }
        public List<GivenCourse> Courses { get; set; }
    }
}
