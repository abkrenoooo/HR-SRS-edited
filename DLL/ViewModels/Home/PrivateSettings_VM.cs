using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Home
{
    public class PrivateSettings_VM
    {
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Pleaze Enter Valid Employee")]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Extra Value")]
        public decimal Extra { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Discount ")]
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
