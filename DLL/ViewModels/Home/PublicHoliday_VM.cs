using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Home
{
    public class PublicHoliday_VM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Extra Value")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
