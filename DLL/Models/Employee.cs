using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string TypeState { get; set; }
        public DateTime BarthDate { get; set; }
        public DateTime DateOfContract { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal Salary { get; set; }
        public string NationalId { get; set; }
        public string Nationality { get; set; }
        public string Notes { get; set; }
    }
}
