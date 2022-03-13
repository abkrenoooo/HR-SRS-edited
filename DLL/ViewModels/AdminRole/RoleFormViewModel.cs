using System.ComponentModel.DataAnnotations;

namespace DLL.ViewModel.AdminRole
{
    public class RoleFormViewModel
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}