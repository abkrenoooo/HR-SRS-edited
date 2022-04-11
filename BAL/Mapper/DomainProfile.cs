using AutoMapper;
using DLL.ViewModels.Employee;
using DLL.ViewModels.Home;
using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DLL.ViewModels;
namespace BAL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Employee, Employee_VM>();
            CreateMap<Employee_VM, Employee>();
            CreateMap<GenralSettings, GenralSettings_VM>();
            CreateMap<GenralSettings_VM, GenralSettings>();
            CreateMap<Attendance_VM, Attendance>();
            CreateMap<Attendance, Attendance_VM>();
            CreateMap<PublicHoliday, PublicHoliday_VM>();
            CreateMap<PublicHoliday_VM, PublicHoliday>();
            CreateMap<PrivateSettings, PrivateSettings_VM>();
            CreateMap<PrivateSettings_VM, PrivateSettings>();
            CreateMap<AbsentDaysAllow, AbsentDaysAllow_VM>();
            CreateMap<AbsentDaysAllow_VM, AbsentDaysAllow>();
            CreateMap<Rewaed, Rewaed_VM>();
            CreateMap<Rewaed_VM, Rewaed>();
            CreateMap<AdvancePayment, AdvancePayment_VM>();
            CreateMap<AdvancePayment_VM, AdvancePayment>();
        }

    }
}
