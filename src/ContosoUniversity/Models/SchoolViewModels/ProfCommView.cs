using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class ProfCommView
    {
        public Professor Professor { set; get; }
        public IEnumerable<Committee> Committees { get; set; }
        public IEnumerable<CommitieMembership> Membership { get; set; }
    }
}
