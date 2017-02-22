using ContosoUniversity.UniversityFunctionalityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
