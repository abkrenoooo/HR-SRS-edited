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
    }
}
