using DLL.Data;
using DLL.Models;
using DLL.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository.SalaryReport
{
    public class Salary : ISalary
    {
        private readonly ApplicationDbContext db;

        public Salary(ApplicationDbContext db)
        {
            this.db = db;
        }
        public static double y = 0;
        public IQueryable<SalaryReport_VM> GetEmployees(int year, int month)
        {
            var publicHolidays = db.publicHolidays.Where(x => x.Date.Year == year && x.Date.Month == month).Count();
            var attendanceDays = db.Attendances.Where(x => x.Date.Year == year && x.Date.Month == month);
            var genralSettings = db.genralSettings.FirstOrDefault();
            int absentDays = 0;
            
            var MonthDays = db.Attendances.Where(a => a.Date.Month == month && a.Date.Year == year);
            for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
            {
                absentDays++;
                var dAttendances = db.Attendances.Where(x => x.Date == date);
                var dpublicHolidays = db.publicHolidays.Where(x => x.Date == date);
                if (dAttendances.Count() > 0 || dpublicHolidays.Count() > 0)
                {
                    absentDays--;
                }
                else if (genralSettings.Satrday == true && date.Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    absentDays--;
                }
                else if (genralSettings.Sunday == true && date.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    absentDays--;
                }
                else if (genralSettings.Monday == true && date.Date.DayOfWeek == DayOfWeek.Monday)
                {
                    absentDays--;
                }
                else if (genralSettings.Tuesday == true && date.Date.DayOfWeek == DayOfWeek.Tuesday)
                {
                    absentDays--;
                }
                else if (genralSettings.wednesday == true && date.Date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    absentDays--;
                }
                else if (genralSettings.Turthday == true && date.Date.DayOfWeek == DayOfWeek.Tuesday)
                {
                    absentDays--;
                }
                else if (genralSettings.Friday == true && date.Date.DayOfWeek == DayOfWeek.Friday)
                {
                    absentDays--;
                }
            }
            var discountHours = db.Attendances.Sum(x => x.StartTime.Minutes);
            var data = db.Employees
           .Select(x => new SalaryReport_VM
           {
               Name = x.Name,
               Phone = x.Phone,
               Id = x.Id,
               Salary = x.Salary,
               AttendanceDays = attendanceDays.Count(),
               AbsentDays = absentDays,
               Discount = genralSettings.Discount * (decimal)DiscountHours( x.StartTime, 1, x.Id, month, year),
               Extra = genralSettings.Extra *(decimal) DiscountHours( x.EndTime, 0, x.Id, month, year),
               DiscountHours = TimeSpan.FromMinutes(DiscountHours( x.StartTime, 1, x.Id, month, year)).ToString(@"hh\:mm"),
               OvertimeHours = TimeSpan.FromMinutes(DiscountHours( x.EndTime, 0, x.Id, month, year)).ToString(@"hh\:mm"),
               Total = x.Salary - ((decimal)DiscountHours( x.EndTime, 0, x.Id, month, year) * genralSettings.Discount / (decimal)60) + ((decimal)DiscountHours( x.EndTime, 1, x.Id, month, year) * genralSettings.Extra / (decimal)60)
           });
            return data;
        }
        public double DiscountHours( TimeSpan Minutes, int q, int id, int month, int year)
        {
            double sum = 0, data = 0;
            var atten = db.Attendances.Where(a => a.EmployeeId == id && a.Date.Month == month && a.Date.Year == year);
            int attendanceDays = atten.Count();
            if (q == 1)
            {
                foreach (var item in atten)
                {
                    var c =item.StartTime.TotalMinutes - Minutes.TotalMinutes ;
                    sum += c ;
                }
                data = sum+y;
                y = 0;
            }
            else if (q == 0)
            {
                y = 0;
                foreach (var item in atten)
                {
                    var c = Minutes .TotalMinutes- item.EndTime.TotalMinutes;
                    if (c >= 0)
                    {
                        y += c;
                    }
                    else
                    {
                        sum += Math.Abs(c);
                    }
                }
                data = sum;
            }
            return Math.Abs(data);
        }

    }
}
