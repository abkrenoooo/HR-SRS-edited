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
        public decimal Discount(int id, int year, int month);
        public decimal Extra(int id, int year, int month);
        public decimal AdvancePayments(int id, int year, int month);
    }
}
