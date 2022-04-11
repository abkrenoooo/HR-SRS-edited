using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Employee
{
    public class Employee_VM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid EMail")]
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Phone")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(11, ErrorMessage = "Pleaze Enter Valid Phone")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"\d*",ErrorMessage = "Entered phone format is not valid.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Pleaze Select Valid Type")]
        public string TypeState { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Barth Date")]
        [DataType(DataType.Date)]
        public DateTime BarthDate { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Date Of Contract")]
        [DataType(DataType.Date)]
        public DateTime DateOfContract { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Salarry")]
        public decimal Salary { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid NationalId")]
        [MinLength(13,ErrorMessage = "The national number must not be less than 14 digits!")]
        public string NationalId { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Nationality")]
        public string Nationality { get; set; }
        public string Notes { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Number")]
        public int AbsentDay { get; set; }
    }
}
