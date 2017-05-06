using ContosoUniversity.Models;
using ContosoUniversity.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Areas.Workflow.Models.CommitteesViews
{
    public class IndexView
    {
        public IEnumerable<Committee> Commitees { set; get; }
        public List<CommitieMembership> MembershipIn { set; get; }
    }
}
