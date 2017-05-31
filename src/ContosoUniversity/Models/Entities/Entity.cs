using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models.Entities
{
    public class Entity
    {
        public bool Archived { set; get; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
