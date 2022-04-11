using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DLL.Models;
using DLL.ViewModels.Employee;
using DLL.Data;

namespace BAL.Repository
{
    public class EmployeeRep : IEmployeeRep
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public EmployeeRep(ApplicationDbContext db, IMapper _Mapper)
        {
            this.db = db;
            mapper = _Mapper;
        }

        public IQueryable<Employee_VM> Get()
        {
            IQueryable<Employee_VM> data = GetAllEmps();
            return data;
        }


        public Employee_VM GetById(int id)
        {
            Employee_VM data = GetEmployeeByID(id);
            return data;
        }


        public void Add(Employee_VM emp)
        {
            // Mapping
            var data = mapper.Map<Employee>(emp);
            db.Employees.Add(data);
            db.SaveChanges();
        }

        public void Edit(Employee_VM emp)
        {
            // Mapping
            var data = mapper.Map<Employee>(emp);
            db.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();

        }

        public void Delete(int id)
        {
            var DeletedObject = db.Employees.Find(id);
            db.Employees.Remove(DeletedObject);
            db.SaveChanges();
        }



        // Refactor
        private Employee_VM GetEmployeeByID(int id)
        {
            return db.Employees.Where(a => a.Id == id)
                                    .Select(a => new Employee_VM { Id = a.Id, Name = a.Name, Salary = a.Salary, Address = a.Address, BarthDate = a.BarthDate, DateOfContract = a.DateOfContract, EMail = a.EMail, Notes = a.Notes, TypeState = a.TypeState, StartTime = a.StartTime, EndTime = a.EndTime, Nationality = a.Nationality, Phone = a.Phone, NationalId = a.NationalId, AbsentDay = a.AbsentDay })
                                    .FirstOrDefault();
        }

        private IQueryable<Employee_VM> GetAllEmps()
        {
            return db.Employees
                       .Select(a => new Employee_VM { Id = a.Id, Name = a.Name, Salary = a.Salary, Address = a.Address, BarthDate = a.BarthDate, DateOfContract = a.DateOfContract, EMail = a.EMail, Notes = a.Notes, TypeState = a.TypeState, StartTime = a.StartTime, EndTime = a.EndTime, Nationality = a.Nationality, Phone = a.Phone, NationalId = a.NationalId, AbsentDay = a.AbsentDay });
        }
        public decimal discount(int id)
        {
            var emp = db.PrivateSettings.Select(x => x.EmployeeId == id).Count();
            if (emp == 0 || db.PrivateSettings.Where(x => x.EmployeeId == id).Count() == 0)
            {
                return db.genralSettings.Select(e => e.Discount).FirstOrDefault();
            }
            else
            {
                return db.PrivateSettings.Where(e => e.EmployeeId == id).Select(e => e.Discount).FirstOrDefault();
            }
        }
        public decimal extra(int id)
        {
            var emp = db.PrivateSettings.Select(x => x.EmployeeId == id).Count();
            if (emp == 0 || db.PrivateSettings.Where(x => x.EmployeeId == id).Count() == 0)
            {
                return db.genralSettings.Select(e => e.Discount).FirstOrDefault();
            }
            else
            {
                return db.PrivateSettings.Where(e => e.EmployeeId == id).Select(e => e.Discount).FirstOrDefault();
            }
        }
        public List<int> attendanceDays(int id, int year, int month)
        {
            var absent = db.absentDaysAllows.Where(x => x.EmployeeId == id && x.Date.Year == year && x.Date.Month == month).Count();
            var list = new List<int>();
            int absentDays = 0;
            var employees = db.Employees.ToList();
            var publicHolidays = db.publicHolidays.Where(x => x.Date.Year == year && x.Date.Month == month).Count();
            int attendanceDays = 0;
            attendanceDays = db.Attendances.Where(x => x.EmployeeId == id && x.Date.Year == year && x.Date.Month == month).Count();
            attendanceDays += absent;
            absentDays -= absent;
            var genralSettings = db.genralSettings.FirstOrDefault();
            var emp = db.PrivateSettings.Select(x => x.EmployeeId == id).Count();
            if (emp == 0 || db.PrivateSettings.Any(x => x.EmployeeId == id) == false)
            {
                absentDays = 0;
                int count = 0;
                for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
                {
                    count++;
                    absentDays++;
                    var dAttendances = db.Attendances.Any(x => x.EmployeeId == id && x.Date == date);
                    var AbsentDay = db.absentDaysAllows.Any(x => x.EmployeeId == id && x.Date == date);
                    var dpublicHolidays = db.publicHolidays.Any(x => x.Date == date);
                    if (dAttendances || dpublicHolidays || AbsentDay)
                    {
                        absentDays--;
                        attendanceDays++;
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
                    else if (genralSettings.Turthday == true && date.Date.DayOfWeek == DayOfWeek.Thursday)
                    {
                        absentDays--;
                    }
                    else if (genralSettings.Friday == true && date.Date.DayOfWeek == DayOfWeek.Friday)
                    {
                        absentDays--;
                    }
                }
                list.Add(absentDays);
                list.Add(count - absentDays);
                return list;
            }
            else
            {
                var e = db.PrivateSettings.Where(x => x.EmployeeId == id).ToList();
                absentDays = 0;
                int count = 0;
                for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
                {
                    count++;
                    absentDays++;
                    var dAttendances = db.Attendances.Any(x => x.EmployeeId == id && x.Date == date);
                    var AbsentDay = db.absentDaysAllows.Any(x => x.EmployeeId == id && x.Date == date);
                    var dpublicHolidays = db.publicHolidays.Any(x => x.Date == date);
                    if (dAttendances || dpublicHolidays || AbsentDay)
                    {
                        absentDays--;
                        attendanceDays++;
                    }
                    else if (e[0].Satrday == true && date.Date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        absentDays--;
                    }
                    else if (e[0].Sunday == true && date.Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        absentDays--;
                    }
                    else if (e[0].Monday == true && date.Date.DayOfWeek == DayOfWeek.Monday)
                    {
                        absentDays--;
                    }
                    else if (e[0].Tuesday == true && date.Date.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        absentDays--;
                    }
                    else if (e[0].wednesday == true && date.Date.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        absentDays--;
                    }
                    else if (e[0].Turthday == true && date.Date.DayOfWeek == DayOfWeek.Thursday)
                    {
                        absentDays--;
                    }
                    else if (e[0].Friday == true && date.Date.DayOfWeek == DayOfWeek.Friday)
                    {
                        absentDays--;
                    }
                }
                list.Add(absentDays);
                list.Add(count - absentDays);
                return list;
            }

        }
        public bool Attendance(Attendance attendance)
        {
            var absentDaysAllow = db.absentDaysAllows.Where(x => x.Date.Day == attendance.Date.Day && x.Date.Month == attendance.Date.Month && x.Date.Year == attendance.Date.Year).Count();
            var publicHolidays = db.publicHolidays.Where(x => x.Date.Day == attendance.Date.Day && x.Date.Month == attendance.Date.Month).Count();
            var emp = db.Employees.Find(attendance.EmployeeId);
            if (emp is null)
            {
                return false;
            }
            var genralSettings = db.genralSettings.FirstOrDefault();
            var count = db.Attendances.Where(x => x.EmployeeId == attendance.EmployeeId && x.Date == attendance.Date && x.Date.Year == attendance.Date.Year).Count();
            if (0 < publicHolidays || absentDaysAllow > 0)
            {
                return false;
            }
            if ((attendance.EndTime <= attendance.StartTime))
            {
                return false;
            }
            if ((emp.StartTime > attendance.StartTime))
            {
                return false;
            }
            if (count > 0)
            {
                return false;
            }
            if (db.PrivateSettings.Any(x => x.EmployeeId == attendance.EmployeeId) == false)
            {
                if (genralSettings.Satrday == true && attendance.Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    return false;
                }
                else if (genralSettings.Sunday == true && attendance.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    return false;
                }
                else if (genralSettings.Monday == true && attendance.Date.DayOfWeek == DayOfWeek.Monday)
                {
                    return false;
                }
                else if (genralSettings.Tuesday == true && attendance.Date.DayOfWeek == DayOfWeek.Tuesday)
                {
                    return false;
                }
                else if (genralSettings.wednesday == true && attendance.Date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    return false;
                }
                else if (genralSettings.Turthday == true && attendance.Date.DayOfWeek == DayOfWeek.Thursday)
                {
                    return false;
                }
                else if (genralSettings.Friday == true && attendance.Date.DayOfWeek == DayOfWeek.Friday)
                {
                    return false;
                }
            }
            else
            {
                var e = db.PrivateSettings.Where(x => x.EmployeeId == attendance.EmployeeId).ToList();
                if (e[0].Satrday == true && attendance.Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    return false;
                }
                else if (e[0].Sunday == true && attendance.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    return false;
                }
                else if (e[0].Monday == true && attendance.Date.DayOfWeek == DayOfWeek.Monday)
                {
                    return false;
                }
                else if (e[0].Tuesday == true && attendance.Date.DayOfWeek == DayOfWeek.Tuesday)
                {
                    return false;
                }
                else if (e[0].wednesday == true && attendance.Date.DayOfWeek == DayOfWeek.Wednesday)
                {
                    return false;
                }
                else if (e[0].Turthday == true && attendance.Date.DayOfWeek == DayOfWeek.Thursday)
                {
                    return false;
                }
                else if (e[0].Friday == true && attendance.Date.DayOfWeek == DayOfWeek.Friday)
                {
                    return false;
                }

            }

            return true;
        }
    }
}
