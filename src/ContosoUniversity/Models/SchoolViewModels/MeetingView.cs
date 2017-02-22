using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class FilesAssosiation
    {
        public FileBase File { get; set; }
        public string Author { set; get; }
    }
    public class MeetingView
    {
        public Meetings Meeting { set; get; }
        public IEnumerable<MeetingComment> PublicComments { set; get; }
        public IEnumerable<MeetingComment> PrivateComments { set; get; }
        public IEnumerable<FilesAssosiation> PrivateFiles { set; get; }
        public IEnumerable<FilesAssosiation> PublicFiles { set; get; }
    }
}
