using ContosoUniversity.Models;
using ContosoUniversity.Models.Entities;
using System.Collections.Generic;

namespace ContosoUniversity.Areas.Workflow.Models.RequestModels
{ 
    public class RequestView
    {
        public List<TeachingRequest> Requests { set; get; }
        public List<Semester> Semesters { set; get; }
        public int UserID { set; get; }
    }
}