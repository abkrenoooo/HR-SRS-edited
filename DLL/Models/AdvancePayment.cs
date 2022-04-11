using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class AdvancePayment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public decimal Value { get; set; }
        public int NumberOfMonthes { get; set; }
        public DateTime Date { get; set; }
    }
}
