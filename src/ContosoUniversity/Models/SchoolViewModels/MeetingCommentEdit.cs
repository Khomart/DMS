using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class MeetingCommentEdit 
    {
        public MeetingComment Comment { set; get; }
        public ICollection<FileBase> Uploaded { set; get; }
    }
}
