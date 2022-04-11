using AutoMapper;
using BAL.Repository;
using BAL.Repository.SalaryReport;
using DLL.Data;
using DLL.Models;
using DLL.ViewModel.AdminRole;
using DLL.ViewModels;
using DLL.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HR_High.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMapper mapper;
        private readonly ISalary salary;
        private readonly IEmployeeRep employee;
        private readonly ILogger<HomeController> _logger;
        public ApplicationDbContext db { get; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db, IMapper _Mapper, ISalary _salary, IEmployeeRep _employee)
        {
            _logger = logger;
            db = _db;
            mapper = _Mapper;
            salary = _salary;
            employee = _employee;
        }


        public IActionResult Index()
        {
            var data = employee.Get();
            ViewBag.Employees = employee.Get().Count();
            return View(data);
        }

        #region Genral Settings
        [Authorize("Permissions.GenralSettings.View")]
        public async Task<IActionResult> GenralSettingsView()
        {
            var result = db.genralSettings.FirstOrDefault();
            if (result == null)
            {
                var genralSettings = new GenralSettings
                {
                    Extra = 100,
                    Discount = 100,
                    Friday = true,
                    Satrday = true,
                    Monday = false,
                    Sunday = false,
                    Tuesday = false,
                    Turthday = false,
                    wednesday = false,
                };
                await db.genralSettings.AddAsync(genralSettings);
                db.SaveChanges();
            }
            result = await db.genralSettings.FirstOrDefaultAsync();

            var data = mapper.Map<GenralSettings_VM>(result);
            return View(data);
        }
        [Authorize("Permissions.GenralSettings.Create")]
        public async Task<IActionResult> GenralSettings()
        {
            var result = await db.genralSettings.FirstOrDefaultAsync();
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

        [HttpPost]
        public async Task<IActionResult> import(IFormFile file)
        {
            try
            {
                ViewBag.Employee = db.Employees.ToList();
                using (var strem = new MemoryStream())
                {
                    await file.CopyToAsync(strem);
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    using (var package = new ExcelPackage(strem))
                    {

                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowacount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowacount; row++)
                        {
                            var attendance=new Attendance
                            {
                                EmployeeId = int.Parse(worksheet.Cells[row, 1].Value.ToString().Trim()),
                                Date = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString()),
                                StartTime = TimeSpan.Parse(worksheet.Cells[row, 4].Value.ToString().Substring(10, 8).Trim()),
                                EndTime = TimeSpan.Parse(worksheet.Cells[row, 5].Value.ToString().Substring(10, 8).Trim()) + new TimeSpan(12, 0, 0),
                            };
                            if (employee.Attendance(attendance))
                            {
                                await db.Attendances.AddAsync(attendance);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return View("Attendance");
            }
            catch (Exception)
            {
                throw;
            }
        }

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
                    var absentDaysAllow = db.absentDaysAllows.Where(x => x.Date.Day == attendance.Date.Day && x.Date.Month == attendance.Date.Month && x.Date.Year == attendance.Date.Year).Count();
                    var publicHolidays = db.publicHolidays.Where(x => x.Date.Day == attendance.Date.Day && x.Date.Month == attendance.Date.Month).Count();
                    var emp = db.Employees.Find(attendance.EmployeeId);
                    var genralSettings = db.genralSettings.FirstOrDefault();
                    var count = db.Attendances.Where(x => x.EmployeeId == attendance.EmployeeId && x.Date == attendance.Date && x.Date.Year == attendance.Date.Year).Count();
                    if (0 < publicHolidays || absentDaysAllow > 0)
                    {
                        return Json("Public Holidays");
                    }
                    if ((attendance.EndTime <= attendance.StartTime))
                    {
                        return Json("EndTime");
                    }
                    if ((emp.StartTime > attendance.StartTime))
                    {
                        return Json("StartTime");
                    }
                    if ((count > 0))
                    {
                        return Json("date");
                    }
                    if (db.PrivateSettings.Where(x => x.EmployeeId == attendance.EmployeeId).Count() == 0)
                    {
                        if (genralSettings.Satrday == true && attendance.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            return Json("holiday");
                        }
                        else if (genralSettings.Sunday == true && attendance.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            return Json("holiday");
                        }
                        else if (genralSettings.Monday == true && attendance.Date.DayOfWeek == DayOfWeek.Monday)
                        {
                            return Json("holiday");
                        }
                        else if (genralSettings.Tuesday == true && attendance.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            return Json("holiday");
                        }
                        else if (genralSettings.wednesday == true && attendance.Date.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            return Json("holiday");
                        }
                        else if (genralSettings.Turthday == true && attendance.Date.DayOfWeek == DayOfWeek.Thursday)
                        {
                            return Json("holiday");
                        }
                        else if (genralSettings.Friday == true && attendance.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            return Json("holiday");
                        }
                    }
                    else
                    {
                        var e = db.PrivateSettings.Where(x => x.EmployeeId == attendance.EmployeeId).ToList();
                        if (e[0].Satrday == true && attendance.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            return Json("holiday");
                        }
                        else if (e[0].Sunday == true && attendance.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            return Json("holiday");
                        }
                        else if (e[0].Monday == true && attendance.Date.DayOfWeek == DayOfWeek.Monday)
                        {
                            return Json("holiday");
                        }
                        else if (e[0].Tuesday == true && attendance.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            return Json("holiday");
                        }
                        else if (e[0].wednesday == true && attendance.Date.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            return Json("holiday");
                        }
                        else if (e[0].Turthday == true && attendance.Date.DayOfWeek == DayOfWeek.Thursday)
                        {
                            return Json("holiday");
                        }
                        else if (e[0].Friday == true && attendance.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            return Json("holiday");
                        }

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
            ViewBag.Employee = db.Employees.ToList();
            return View();
        }
        [Authorize("Permissions.Salaryreport.View")]
        public IActionResult LoadDateSalaryReport(int year, int month, int EmployeeId)
        {
            int DayCount = 0;
            for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
            {
                DayCount++;
            }
            if (EmployeeId ==0)
            {
                var data = db.Employees
               .Select(x => new SalaryReport_VM
               {
                   Name = x.Name,
                   Phone = x.Phone,
                   Id = x.Id,
                   Salary = x.Salary,
                   AttendanceDays = employee.attendanceDays(x.Id, year, month)[1],
                   AbsentDays = employee.attendanceDays(x.Id, year, month)[0],
                   Discount = ((decimal)salary.DiscountHours(x.EndTime, 0, x.Id, month, year) * employee.discount(x.Id) / (decimal)60) + salary.Extra(x.Id, year, month),
                   Extra = ((decimal)salary.DiscountHours(x.StartTime, 1, x.Id, month, year) * employee.extra(x.Id) / (decimal)60) + salary.Discount(x.Id, year, month),
                   DiscountHours = TimeSpan.FromMinutes(salary.DiscountHours(x.EndTime, 0, x.Id, month, year)).ToString(@"hh\:mm"),
                   OvertimeHours = TimeSpan.FromMinutes(salary.DiscountHours(x.StartTime, 1, x.Id, month, year)).ToString(@"hh\:mm"),
                   AdvancePayments = salary.AdvancePayments(x.Id, year, month),
                   Total = x.Salary + (((decimal)salary.DiscountHours(x.EndTime, 0, x.Id, month, year) * employee.discount(x.Id) / (decimal)60) + salary.Extra(x.Id, year, month)) - (((decimal)salary.DiscountHours(x.StartTime, 1, x.Id, month, year) * employee.discount(x.Id) / (decimal)60) + salary.Discount(x.Id, year, month)) - ((x.Salary / DayCount) * employee.attendanceDays(x.Id, year, month)[0]) - salary.AdvancePayments(x.Id, year, month),
               });
                return Json(data);
            }
            else
            {
                var data = db.Employees.Where(x => x.Id == EmployeeId)
               .Select(x => new SalaryReport_VM
               {
                   Name = x.Name,
                   Phone = x.Phone,
                   Id = x.Id,
                   Salary = x.Salary,
                   AttendanceDays = employee.attendanceDays(x.Id, year, month)[1],
                   AbsentDays = employee.attendanceDays(x.Id, year, month)[0],
                   Discount = ((decimal)salary.DiscountHours(x.EndTime, 0, x.Id, month, year) * employee.discount(x.Id) / (decimal)60) + salary.Extra(x.Id, year, month),
                   Extra = ((decimal)salary.DiscountHours(x.StartTime, 1, x.Id, month, year) * employee.extra(x.Id) / (decimal)60) + salary.Discount(x.Id, year, month),
                   DiscountHours = TimeSpan.FromMinutes(salary.DiscountHours(x.EndTime, 0, x.Id, month, year)).ToString(@"hh\:mm"),
                   OvertimeHours = TimeSpan.FromMinutes(salary.DiscountHours(x.StartTime, 1, x.Id, month, year)).ToString(@"hh\:mm"),
                   AdvancePayments = salary.AdvancePayments(x.Id, year, month),
                   Total = x.Salary + (((decimal)salary.DiscountHours(x.EndTime, 0, x.Id, month, year) * employee.discount(x.Id) / (decimal)60) + salary.Extra(x.Id, year, month)) - (((decimal)salary.DiscountHours(x.StartTime, 1, x.Id, month, year) * employee.extra(x.Id) / (decimal)60) + salary.Discount(x.Id, year, month)) - salary.AdvancePayments(x.Id, year, month)
               });
                return Json(data);
            }
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
                    if (countName != null && countDate != null)
                    {
                        if (countName.Id != publicHoliday.Id)
                        {
                            ModelState.AddModelError("Name", "This Name already exists");
                            return View(publicHoliday);


                        }

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
        public IActionResult Invoice()
        {
            return View();
        }
        #endregion

        #region Private  Settings
        [Authorize("Permissions.PrivateSettings.Create")]
        public IActionResult PrivateSettings()
        {
            ViewBag.Employee = db.Employees.ToList();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> FindPrivateSettingsByEmployeeID(int id)
        {
            var emp = await db.PrivateSettings.Where(x => x.EmployeeId == id).Select(x => new PrivateSettings_VM { Id = x.Id, Discount = x.Discount, Extra = x.Extra, Satrday = x.Satrday, Sunday = x.Sunday, Friday = x.Friday, Monday = x.Monday, Tuesday = x.Tuesday, Turthday = x.Turthday, wednesday = x.wednesday }).SingleOrDefaultAsync();

            return Json(emp);
        }

        [Authorize("Permissions.PrivateSettings.Create")]
        [HttpPost]
        public async Task<IActionResult> PrivateSettings(PrivateSettings_VM input)
        {
            ViewBag.Employee = await db.Employees.ToListAsync();
            try
            {
                if (ModelState.IsValid)
                {
                    var emp = db.PrivateSettings.Where(x => x.EmployeeId == input.EmployeeId).Count();
                    if (emp == 0)
                    {
                        var data = mapper.Map<PrivateSettings>(input);
                        db.PrivateSettings.Add(data);
                        db.SaveChanges();
                        return Json(true);
                    }
                    else
                    {
                        // var empPrivate = await db.PrivateSettings.Where(x => x.EmployeeId == input.EmployeeId).SingleOrDefaultAsync();
                        //      input.Id = empPrivate.Id;
                        var data = mapper.Map<PrivateSettings>(input);
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(true);
                    }
                }
                else
                {
                    return Json(false);
                }
            }
            catch
            {
                return Json(false);
            }
        }
        #endregion

        #region AbsentDaysAllow
        [Authorize("Permissions.AbsentDaysAllow.View")]
        public IActionResult AbsentDaysAllow()
        {
            ViewBag.Employee = db.Employees.ToList();
            return View();
        }
        [Authorize("Permissions.AbsentDaysAllow.Create")]
        [HttpPost]
        public IActionResult CreateAbsentDaysAllow(AbsentDaysAllow absentDaysAllow)
        {
            ViewBag.Employee = db.Employees.ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    var publicHolidays = db.publicHolidays.Where(x => x.Date.Day == absentDaysAllow.Date.Day && x.Date.Month == absentDaysAllow.Date.Month && x.Date.Year == absentDaysAllow.Date.Year).Count();
                    var attendance = db.absentDaysAllows.Where(x => x.Date.Day == absentDaysAllow.Date.Day && x.Date.Month == absentDaysAllow.Date.Month && x.Date.Year == absentDaysAllow.Date.Year).Count();
                    var emp = db.Employees.Find(absentDaysAllow.EmployeeId);
                    var empAbsent = db.absentDaysAllows.Where(x => x.EmployeeId == absentDaysAllow.EmployeeId).Select(x => x.AbsentDays).Count();
                    var genralSettings = db.genralSettings.FirstOrDefault();
                    var count = db.Attendances.Where(x => x.EmployeeId == absentDaysAllow.EmployeeId && x.Date == absentDaysAllow.Date).Count();
                    if (0 < publicHolidays || count > 0 || empAbsent == emp.AbsentDay)
                    {
                        ModelState.AddModelError("Date", "This Date Is Found In Public Holidays or in Attendance");
                        return Json(false);
                    }
                    if ((attendance > 0))
                    {
                        ModelState.AddModelError("Date", "This date already exists");
                        return Json(false);
                    }
                    var empDays = db.Employees.Where(x => x.Id == absentDaysAllow.EmployeeId).Select(x => x.AbsentDay).Count();
                    var privateSet = db.PrivateSettings.Where(x => x.EmployeeId == absentDaysAllow.EmployeeId).Count();
                    if (empDays > 0 && privateSet == 0)
                    {
                        if (genralSettings.Satrday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (genralSettings.Sunday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (genralSettings.Monday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Monday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (genralSettings.Tuesday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (genralSettings.wednesday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (genralSettings.Turthday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (genralSettings.Friday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                    }
                    else
                    {
                        var e = db.PrivateSettings.Where(x => x.EmployeeId == absentDaysAllow.EmployeeId).ToList();
                        if (e[0].Satrday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (e[0].Sunday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (e[0].Monday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Monday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (e[0].Tuesday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (e[0].wednesday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (e[0].Turthday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }
                        else if (e[0].Friday == true && absentDaysAllow.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            ModelState.AddModelError("Date", "This date already exists");
                            return Json(false);
                        }

                    }
                    var data = mapper.Map<AbsentDaysAllow>(absentDaysAllow);
                    db.absentDaysAllows.Add(data);
                    absentDaysAllow.AbsentDays++;
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
        [Authorize("Permissions.AbsentDaysAllow.View")]
        public IActionResult LoadDateabAentDaysAllow()
        {

            var data = db.absentDaysAllows
                .Select(x => new AbsentDaysAllow_VM { Date = x.Date, EmployeeId = x.EmployeeId, Id = x.Id, EmployeeName = x.Employee.Name }).ToList();
            return Json(data);
        }
        [Authorize("Permissions.Attendance.Delete")]

        public IActionResult DeleteAbsentDaysAllow(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.absentDaysAllows.Find(id);
                    db.absentDaysAllows.Remove(data);
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

        #region AdvancePayment
        [Authorize("Permissions.AdvancePayment.View")]
        public IActionResult AdvancePayment()
        {
            ViewBag.Employee = db.Employees.ToList();
            return View();
        }
        [Authorize("Permissions.AdvancePayment.Create")]
        [HttpPost]
        public IActionResult CreateAdvancePayment(AdvancePayment_VM advancePayment)
        {
            ViewBag.Employee = db.Employees.ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    var emp = db.Employees.Where(x => x.Id == advancePayment.EmployeeId).Sum(x => x.Salary);
                    var allAdvancePayment = db.AdvancePayments.Where(x => x.EmployeeId == advancePayment.EmployeeId && x.Date.Month == advancePayment.Date.Month && x.Date.Year == advancePayment.Date.Year).Sum(x => x.Value);
                    if (advancePayment.Value > (emp - allAdvancePayment))
                    {
                        ModelState.AddModelError("Value", "This Value may be bigger than him Salary");
                        return Json("Value");
                    }
                    if (advancePayment.Id == 0)
                    {
                        var data = mapper.Map<AdvancePayment>(advancePayment);
                        db.AdvancePayments.Add(data);
                    }
                    else
                    {
                        var data = mapper.Map<AdvancePayment>(advancePayment);
                        db.Entry(data).State = EntityState.Modified;
                    }
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
        [Authorize("Permissions.AdvancePayment.View")]
        public IActionResult LoadDateAdvancePayment()
        {

            var data = db.AdvancePayments
                .Select(x => new AdvancePayment_VM { Date = x.Date, EmployeeId = x.EmployeeId, NumberOfMonthes = x.NumberOfMonthes, Value = x.Value, Id = x.Id, EmployeeName = x.Employee.Name }).ToList();
            return Json(data);
        }
        [Authorize("Permissions.AdvancePayment.Delete")]

        public IActionResult DeleteAdvancePayment(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.AdvancePayments.Find(id);
                    db.AdvancePayments.Remove(data);
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
        [Authorize("Permissions.AdvancePayment.Edit")]
        public IActionResult EditAdvancePayment(int id)
        {
            ViewBag.Employee = db.Employees.ToList();
            var advance = db.AdvancePayments.Find(id);
            var data = new AdvancePayment_VM { Date = advance.Date, EmployeeId = advance.EmployeeId, NumberOfMonthes = advance.NumberOfMonthes, Value = advance.Value, Id = advance.Id, EmployeeName = advance.Employee.Name };

            return View(data);
        }
        #endregion

        #region Rewaed
        [Authorize("Permissions.Rewaed.View")]
        public IActionResult Rewaed()
        {
            ViewBag.Employee = db.Employees.ToList();
            return View();
        }
        [Authorize("Permissions.Rewaed.Create")]
        [HttpPost]
        public IActionResult CreateRewaed(Rewaed_VM rewaed)
        {
            ViewBag.Employee = db.Employees.ToList();
            try
            {
                if (ModelState.IsValid)
                {

                    if (rewaed.Id == 0)
                    {
                        var data = mapper.Map<Rewaed>(rewaed);
                        db.Rewaeds.Add(data);
                    }
                    else
                    {
                        var data = mapper.Map<Rewaed>(rewaed);
                        db.Entry(data).State = EntityState.Modified;
                    }
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
        [Authorize("Permissions.Rewaed.View")]
        public IActionResult LoadDateRewaed()
        {

            var data = db.Rewaeds
                .Select(x => new Rewaed_VM { Date = x.Date, EmployeeId = x.EmployeeId, Type = x.Type, Value = x.Value, Id = x.Id, EmployeeName = x.Employee.Name }).ToList();
            return Json(data);
        }
        [Authorize("Permissions.Rewaed.Delete")]

        public IActionResult DeleteRewaed(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = db.Rewaeds.Find(id);
                    db.Rewaeds.Remove(data);
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
        [Authorize("Permissions.Rewaed.Edit")]
        public IActionResult EditRewaed(int id)
        {
            ViewBag.Employee = db.Employees.ToList();
            var advance = db.Rewaeds.Find(id);
            var data = new Rewaed_VM { Date = advance.Date, EmployeeId = advance.EmployeeId, Type = advance.Type, Value = advance.Value, Id = advance.Id, EmployeeName = advance.Employee.Name };

            return View(data);
        }
        #endregion
    }
}
