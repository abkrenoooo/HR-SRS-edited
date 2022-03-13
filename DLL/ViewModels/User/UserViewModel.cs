using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DLL.ViewModel.User
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Pleaze Enter Valid Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Pleaze Enter Valid UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Pleaze Enter Valid Mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pleaze Enter Valid Password ")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        [Required(ErrorMessage = "Pleaze Select The Group ")]
        public string RoleId { get; set; }

        public IEnumerable<string> Role { get; set; }
    }
}