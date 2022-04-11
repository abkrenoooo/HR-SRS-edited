using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Home
{
    public class AbsentDaysAllow_VM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }
}
