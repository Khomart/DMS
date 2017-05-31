using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Areas.Workflow.Models.AcademicYear
{
    public class Year
    {
        public int YearValue { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Summer1 { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Summer1End { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Summer2 { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Summer2End { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Summer_long { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Summer_longEnd { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Fall { set; get; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime FallEnd { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Winter { set; get; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime WinterEnd { set; get; }
    }
}
