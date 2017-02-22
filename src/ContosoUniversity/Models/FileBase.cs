using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public enum Ownership
    {
        [Display(Name = "Course File")]
        course = 1,
        [Display(Name = "Meeting Public File")]
        meetingPub = 2,
        [Display(Name = "Meeting Private File")]
        meetingPriv = 3,

    }
    public class FileBase
    {

        [Required]
        public string Location { get; set; }
        [Required]
        public string ViewTitle { get; set; }
        public DataType Type { get; set; }
        public int Size { get; set; }
        public DateTime Added { get; set; }
        public DateTime Modified { get; set; }

        [Required]
        public Ownership Owned { set; get; }
        [Required]
        public int OwnerID { set; get; }
        [Required]
        [Key]
        public int FileBaseID { set; get; }
    }
}
