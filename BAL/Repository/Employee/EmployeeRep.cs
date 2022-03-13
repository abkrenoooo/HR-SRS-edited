using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using HR_High.Data;
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
                                    .Select(a => new Employee_VM { Id = a.Id, Name = a.Name, Salary = a.Salary, Address = a.Address, BarthDate = a.BarthDate, DateOfContract = a.DateOfContract, EMail = a.EMail, Notes = a.Notes, TypeState = a.TypeState, StartTime = a.StartTime , EndTime = a.EndTime , Nationality = a.Nationality , Phone = a.Phone ,NationalId=a.NationalId })
                                    .FirstOrDefault();
        }

        private IQueryable<Employee_VM> GetAllEmps()
        {
            return db.Employees
                       .Select(a => new Employee_VM { Id = a.Id, Name = a.Name, Salary = a.Salary, Address = a.Address, BarthDate = a.BarthDate, DateOfContract = a.DateOfContract, EMail = a.EMail, Notes = a.Notes, TypeState = a.TypeState, StartTime = a.StartTime, EndTime = a.EndTime, Nationality = a.Nationality, Phone = a.Phone, NationalId = a.NationalId });
        }
    }
}
