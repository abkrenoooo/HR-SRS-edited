using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModels.Home
{
    public class GenralSettings_VM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Extra Value")]
        public int Extra { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Discount ")]
        public int Discount { get; set; }
        public bool Satrday { get; set; }
        public bool Sunday   { get; set; }
        public bool Monday    { get; set; }
        public bool Tuesday   { get; set; }
        public bool wednesday { get; set; }
        public bool Turthday   { get; set; }
        public bool Friday    { get; set; }

    }
}
