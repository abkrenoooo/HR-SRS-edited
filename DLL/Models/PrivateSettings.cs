using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class PrivateSettings
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public decimal Extra { get; set; }
        public decimal Discount { get; set; }
        public bool Satrday { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool wednesday { get; set; }
        public bool Turthday { get; set; }
        public bool Friday { get; set; }
    }
}
