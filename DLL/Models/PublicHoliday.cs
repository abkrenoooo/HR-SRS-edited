using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Models
{
    public class PublicHoliday
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public DateTime Date { get; set; }
    }
}
