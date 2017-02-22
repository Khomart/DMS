using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class CommitteeInvitation
    {
        public int CommitteeID { get; set; }
        public  IEnumerable<Professor> Professors { get; set; }
        public  IEnumerable<CommitieMembership> Memberships { get; set; }

    }
}
