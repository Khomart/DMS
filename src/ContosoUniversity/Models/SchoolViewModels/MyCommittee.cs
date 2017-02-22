using System.Collections.Generic;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class MyCommittee
    {
        public Committee Committee { get; set; }
        public IEnumerable<CommitieMembership> Members { get; set; }
        public IEnumerable<Meetings> Meetings { get; set; }
    }
}
