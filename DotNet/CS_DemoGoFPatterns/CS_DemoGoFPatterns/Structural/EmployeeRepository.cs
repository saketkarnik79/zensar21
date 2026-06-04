using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> employees= new ();

        public IEnumerable<Employee> GetAll() => employees;

        public Employee GetById(int id) => employees.FirstOrDefault(e => e.Id == id)!;

        public void Add(Employee employee) => employees.Add(employee);
    }
}
