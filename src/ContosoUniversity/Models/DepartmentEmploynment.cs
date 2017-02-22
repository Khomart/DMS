using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class DepartmentEmploynment
    {
        public int DepartmentID { set; get; }
        public int ProfessorID { get; set; }
        public Department Department { get; set; }
        public Professor Professor { get; set; }


    }
}
