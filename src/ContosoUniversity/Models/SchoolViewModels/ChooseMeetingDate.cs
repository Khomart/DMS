using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class ChooseMeetingDate
    {
        public ICollection<CommitieMembership> Members { set; get; }
        public List<DatesSuggestion> Dates { set; get; }
    }
}
