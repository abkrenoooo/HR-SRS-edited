using DLL.ViewModel.AdminRole;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DLL.ViewModel.User
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<CheckBoxViewModel> Roles { get; set; }
    }
}