//form for course request
namespace ContosoUniversity.Models.SchoolViewModels
{
    public class ChoosenCourse : GivenCourse
    {
        public Desire Choice { get; set; }

        //public Semester CourseSemester { get; set; }
    }
}
