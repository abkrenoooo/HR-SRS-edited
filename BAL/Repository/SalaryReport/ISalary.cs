using DLL.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.SalaryReport
{
    public interface ISalary
    {
        public IQueryable<SalaryReport_VM> GetEmployees(int year, int month);
        public double DiscountHours( TimeSpan Minutes,int q, int id, int month, int year);
    }
}
