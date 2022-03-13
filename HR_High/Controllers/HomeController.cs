using AutoMapper;
using BAL.Repository.SalaryReport;
using DLL.Data;
using DLL.Models;
using DLL.ViewModel.AdminRole;
using DLL.ViewModels;
using DLL.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HR_High.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISalary salary;
        private readonly ILogger<HomeController> _logger;
        public ApplicationDbContext db { get; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db, IMapper _Mapper, ISalary _salary)
        {
            _logger = logger;
            db = _db;
            mapper = _Mapper;
            salary = _salary;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Genral Settings
        [Authorize("Permissions.GenralSettings.View")]
        public IActionResult GenralSettingsView()
        {
            var result = db.genralSettings.FirstOrDefault();
            var data = mapper.Map<GenralSettings_VM>(result);
            return View(data);
        }
        [Authorize("Permissions.GenralSettings.Create")]
        public IActionResult GenralSettings()
        {
            var result = db.genralSettings.FirstOrDefault();
            var data = mapper.Map<GenralSettings_VM>(result);
            return View(data);
        }
        [Authorize("Permissions.GenralSettings.Create")]
        [HttpPost]
        public IActionResult GenralSettings(GenralSettings_VM input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = mapper.Map<GenralSettings>(input);
                    db.genralSettings.Update(data);
                    db.SaveChanges();
                    return View("GenralSettingsView");
                }
                else
                {
                    return View(input);
                }
            }
            catch
            {
                return View(input);
            }
        }
        #endregion

        #region Attendance
        [Authorize("Permissions.Attendance.View")]
        public IActionResult Attendance()
        {
            ViewBag.Employee = db.Employees.ToList();
            return View();
        }
        [Authorize("Permissions.Attendance.Create")]
        [HttpPost]
        public IActionResult CreateAttendance(Attendance_VM attendance)
        {
            ViewBag.Employee = db.Employees.ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    var emp = db.Employees.Find(attendance.EmployeeId);
                    var genralSettings = db.genralSettings.FirstOrDefault();
                    var count = db.Attendances.Where(x => x.EmployeeId == attendance.EmployeeId && x.Date == attendance.Date).Count();
                    if ((attendance.EndTime <= attendance.StartTime))
                    {
                        ModelState.AddModelError("EndTime", "End Time Not valid");
                        return Json(false);
                    }
                    if ((emp.StartTime > attendance.StartTime))
                    {
                        ModelState.AddModelError("EndTime", "End Time Not valid");
                        return Json(false);
                    }
                    if ((count > 0))
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    if (attendance.Date.DayOfWeek==DayOfWeek.Saturday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.Satrday == true && attendance.Date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.Sunday == true && attendance.Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.Monday == true && attendance.Date.DayOfWeek == DayOfWeek.Monday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.Tuesday == true && attendance.Date.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.wednesday == true && attendance.Date.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.Turthday == true && attendance.Date.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    else if (genralSettings.Friday == true && attendance.Date.DayOfWeek == DayOfWeek.Friday)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    var data = mapper.Map<Attendance>(attendance);
                    db.Attendances.Add(data);
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                return Json(false);
            }
            catch (Exception)
            {

                return Json(false);
            }

        }
        [Authorize("Permissions.Attendance.View")]
        public IActionResult LoadDateAttendance()
        {

            var data = db.Attendances
                .Select(x => new Attendance_VM { Date = x.Date, EmployeeId = x.EmployeeId, EndTimeStr = x.EndTime.ToString(), StartTimeSTr = x.StartTime.ToString(), Id = x.Id, EmployeeName = x.Employee.Name }).ToList();
            return Json(data);
        }
        [Authorize("Permissions.Attendance.Delete")]
        public IActionResult DeleteAttendance(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.Attendances.Find(id);
                    db.Attendances.Remove(data);
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                return Json(false);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        #endregion

        #region Salary Report
        [Authorize("Permissions.Salaryreport.View")]
        public IActionResult SalaryReport()
        {
            return View();
        }
        [Authorize("Permissions.Salaryreport.View")]
        public IActionResult LoadDateSalaryReport(int year, int month)
        {
            var publicHolidays = db.publicHolidays.Where(x => x.Date.Year == year && x.Date.Month == month).Count();
            var attendanceDays = db.Attendances.Where(x => x.Date.Year == year && x.Date.Month == month);
            var genralSettings = db.genralSettings.FirstOrDefault();
            int absentDays = 0;
            if (attendanceDays.Count() == 0)
            {
                return View();
            }
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
                   Discount = (decimal)salary.DiscountHours( x.EndTime, 0, x.Id, month, year) * genralSettings.Discount / (decimal)60,
                   Extra = (decimal)salary.DiscountHours( x.StartTime, 1, x.Id, month, year) * genralSettings.Extra / (decimal)60,
                   DiscountHours = TimeSpan.FromMinutes(salary.DiscountHours( x.EndTime, 0, x.Id, month, year)).ToString(@"hh\:mm"),
                   OvertimeHours = TimeSpan.FromMinutes(salary.DiscountHours( x.StartTime, 1, x.Id, month, year)).ToString(@"hh\:mm"),
                   Total = x.Salary + (((decimal)salary.DiscountHours( x.EndTime,0, x.Id, month, year) * genralSettings.Discount / (decimal)60) ) - (((decimal)salary.DiscountHours(x.StartTime, 1, x.Id, month, year) * genralSettings.Extra / (decimal)60))
               });
            return Json(data);
        }
        #endregion

        #region Public Holiday
        [Authorize("Permissions.OfficialVacations.View")]
        public IActionResult PublicHoliday()
        {
            return View();
        }
        [Authorize("Permissions.OfficialVacations.View")]
        public IActionResult LoadDatePublicHoliday()
        {
            var data = db.publicHolidays
                .Select(x => new PublicHoliday_VM { Date = x.Date, Name = x.Name, Id = x.Id }).ToList();
            return Json(data);
        }
        [Authorize("Permissions.OfficialVacations.Create")]
        public IActionResult CreatePublicHoliday(PublicHoliday_VM publicHoliday)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var countDate = db.publicHolidays.Where(x => x.Date == publicHoliday.Date).Count();
                    var countName = db.publicHolidays.Where(x => x.Name == publicHoliday.Name).Count();
                    if (countDate > 0 || countName > 0)
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json("date");
                    }
                    var data = mapper.Map<PublicHoliday>(publicHoliday);
                    db.publicHolidays.Add(data);
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                return Json(false);
            }
            catch (Exception)
            {

                return Json(false);
            }
        }
        [Authorize("Permissions.OfficialVacations.Edit")]
        public IActionResult EditPublicHoliday(int id)
        {
            var data = db.publicHolidays.Find(id);
            var publicHoliday = mapper.Map<PublicHoliday_VM>(data);
            return View(publicHoliday);
        }
        [HttpPost]
        public IActionResult EditPublicHoliday(PublicHoliday_VM publicHoliday)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Id = db.publicHolidays.Find(publicHoliday.Id);
                    if (Id == null)
                        return NotFound();
                    var countDate = db.publicHolidays.FirstOrDefault(x => x.Date == publicHoliday.Date);
                    var countName = db.publicHolidays.FirstOrDefault(x => x.Name == publicHoliday.Name);
                    if (countDate != null && countDate.Id != publicHoliday.Id)
                    {
                        ModelState.AddModelError("Date", "This Date already exists");
                        return View(publicHoliday);
                    }
                    if (countName != null && countDate.Id != publicHoliday.Id)
                    {
                        ModelState.AddModelError("Name", "This Name already exists");
                        return View(publicHoliday);
                    }
                    var data = mapper.Map<PublicHoliday>(publicHoliday);
                    Id.Id = publicHoliday.Id;
                    Id.Name = publicHoliday.Name;
                    Id.Date = publicHoliday.Date;
                    //db.Entry(data).State = EntityState.Modified;
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        return RedirectToAction("PublicHoliday", "Home");
                    }
                    else
                    {
                        return View(publicHoliday);
                    }
                }
                return View(publicHoliday);
            }
            catch (Exception)
            {
                return View(publicHoliday);
            }
        }
        [Authorize("Permissions.OfficialVacations.Delete")]
        public IActionResult DeletePublicHoliday(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.publicHolidays.Find(id);
                    db.publicHolidays.Remove(data);
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                return Json(false);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        #endregion

        #region Invoice
        public IActionResult Invoice(SalaryReport_VM salaryReport)
        {
            return View(salaryReport);
        }
        #endregion
    }

}
