using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoOOP.ReferenceTypes
{
    internal class Employee : Person
    {
        public int EmployeeId { get; set; }
        public int DeptNo { get; set; }
        public decimal Salary { get; set; }

        public Employee() : base() // Call the base class default constructor
        {
            EmployeeId = 0;
            DeptNo = 0;
            Salary = 0m;
        }

        public Employee(int id, string name, DateTime dateOfBirth, int employeeId, int deptNo, decimal salary)
            : base(id, name, dateOfBirth) // Call the base class parameterized constructor
        {
            EmployeeId = employeeId;
            DeptNo = deptNo;
            Salary = salary;
        }

        public override string Display()
        {
            return $"{base.Display()}, EmployeeId={EmployeeId}, DeptNo={DeptNo}, Salary={Salary}";
        }
    }
}
