using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class AbsentDaysAllow
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public int AbsentDays { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
