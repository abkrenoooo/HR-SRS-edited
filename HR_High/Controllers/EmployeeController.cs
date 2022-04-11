using AutoMapper;
using BAL.Repository;
using DLL.Data;
using DLL.Models;
using DLL.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR_High.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public IEmployeeRep employee { get; }

        public EmployeeController(ApplicationDbContext db, IMapper _Mapper, IEmployeeRep _employee)
        {
            this.db = db;
            mapper = _Mapper;
            employee = _employee;
        }
        // GET: EmployeeController
        [Authorize("Permissions.Employee.View")]
        public IActionResult Index(int PageIndex, int PageSize)
        {
            var data = employee.Get();

            return View(data);
        }
        [Authorize("Permissions.Employee.Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee_VM emp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (emp.TypeState.ToString() == "Choose Type")
                    {
                        ModelState.AddModelError("TypeState", "Pleaze Select Valid Type");
                        return View(emp);
                    }
                    if (((DateTime.Today - emp.BarthDate.Date).Days / 365) < 20)
                    {
                        ModelState.AddModelError("BarthDate", "Employee must not be less than 20 years old");
                        return View(emp);
                    }
                    if ((emp.DateOfContract < DateTime.Parse("1/1/2008")))
                    {
                        ModelState.AddModelError("DateOfContract", "Please enter a valid contract date");
                        return View(emp);
                    }
                    if ((emp.EndTime <= emp.StartTime))
                    {
                        ModelState.AddModelError("EndTime", "End Time Not valid");
                        return View(emp);
                    }
                    if (emp.Id==0)
                    {
                        employee.Add(emp);
                    }
                    else
                    {
                        employee.Edit(emp);
                    }

                    return RedirectToAction("Index", "Employee");
                }
                else
                {
                    return View(emp);
                }
            }
            catch
            {
                return View(emp);
            }

        }
        [Authorize("Permissions.Employee.Edit")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = employee.GetById(id);
            return View(data);
        }

        
        [Authorize("Permissions.Employee.Delete")]
        public IActionResult Delete(int id)
        {
            var data = employee.GetById(id);
            return View(data);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult ConfirmDelete(int id)
        {
            try
            {
                var emp = db.Employees.Find(id);
                db.Employees.Remove(emp);
                var date = db.SaveChanges();
                if (date > 0)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                var data = employee.GetById(id);
                return View(data);
            }
        }
    }
}
