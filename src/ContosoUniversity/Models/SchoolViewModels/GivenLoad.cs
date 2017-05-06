
using ContosoUniversity.Models.Entities;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class GivenLoad
    {
        public Course Course { set; get; }
        public bool Requested { set; get; }
        public bool Given { get; set; }
        public Semester Semester { set; get; }
    }
}
