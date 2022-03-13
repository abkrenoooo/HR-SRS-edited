﻿using DLL.ViewModel.AdminRole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BAL.Contants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsList(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        }

        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();

            var modules = Enum.GetValues(typeof(Modules));

            foreach (var module in modules)
                allPermissions.AddRange(GeneratePermissionsList(module.ToString()));

            return allPermissions;
        }

        public static class Module
        {
            public static string View(string module)
            {
                return $"Permissions.{module}.View";
            }
            public static string Create(string module)
            {
                return $"Permissions.{module}.Create";
            }
            public static string Edit(string module)
            {
                return $"Permissions.{module}.Edit";
            }
            public static string Delete(string module)
            {
                return $"Permissions.{module}.Delete";
            }
        }
        public static PermissionsFormViewModel ReturnPermissionsFormViewModel(PermissionsFormViewModel model)
        {
            var viewModel = new PermissionsFormViewModel
            {
                RoleId = model.RoleId,
                RoleName = model.RoleName,
                RoleCalims = model.RoleCalims.Select(p => new CheckBoxViewModel { DisplayValue = p.DisplayValue, IsSelected = p.IsSelected }).ToList()
            };
            return viewModel;
        }
    }
}