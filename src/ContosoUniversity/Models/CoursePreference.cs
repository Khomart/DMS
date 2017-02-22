using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class CoursePreference
    {
        public int RequestID { get; set; }
        public int CourseID { get; set; }
        [ForeignKey("RequestID")]
        public TeachingRequest Request { get; set; }
        [ForeignKey("CourseID")]
        public Course Course { get; set; }

        public Desire Choice { get; set; }
    }
}
