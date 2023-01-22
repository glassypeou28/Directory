using System;
using System.Collections.Generic;
using Project.Models;

namespace Project.ViewModels.Maintenance
{
    public class MaintenanceViewModel
    {
        public Employees EmployeeList = new Employees();
        public List<DivisionDepartment> DivDeptList = new List<DivisionDepartment>();
        public List<PersonType> PersonTypeList = new List<PersonType>();
    }
}