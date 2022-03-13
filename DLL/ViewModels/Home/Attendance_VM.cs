using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels
{
    public  class Attendance_VM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Date Of Contract")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Range(1,int.MaxValue, ErrorMessage = "Pleaze Enter Valid Employee")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
        public string StartTimeSTr { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
        public string EndTimeStr { get; set; }
    }
}
