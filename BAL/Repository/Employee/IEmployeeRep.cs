using DLL.Models;
using DLL.ViewModels.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public interface IEmployeeRep
    {
        IQueryable<Employee_VM> Get();
        Employee_VM GetById(int id);
        void Add(Employee_VM emp);
        void Edit(Employee_VM emp);
        void Delete(int id);
        public decimal discount(int id);
        public decimal extra(int id);
        public List<int> attendanceDays(int id, int year, int month);
        public bool Attendance(Attendance attendance);
    }
}
