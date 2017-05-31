using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.SchoolViewModels
{
    public class EditProf : RegProfModel
    {
        [Required]
        [Key]
        public int Id { set; get; }
        public string FullName { get
            {
                return FirstMidName + " " + LastName;
            } }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
