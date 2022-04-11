using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Home
{
    public class AdvancePayment_VM
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Pleaze Enter Valid Employee")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Pleaze Enter Valid Value")]
        public decimal Value { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Pleaze Enter Valid Value")]
        public int NumberOfMonthes { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Date Of Contract")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
