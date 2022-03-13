using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Home
{
    public class SalaryReport_VM
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public decimal Salary { get; set; }
        public int AttendanceDays { get; set; }
        public int AbsentDays { get; set; }
        public string OvertimeHours { get; set; }
        public string DiscountHours { get; set; }
        public decimal Extra { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
