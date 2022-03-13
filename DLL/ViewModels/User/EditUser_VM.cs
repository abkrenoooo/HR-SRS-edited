using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.ViewModel.User
{
    public class EditUser_VM
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Pleaze Enter Valid UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Pleaze Enter Valid Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pleaze Select The Group ")]
        public string RoleId { get; set; }

        public IEnumerable<string> Role { get; set; }
    }
}
