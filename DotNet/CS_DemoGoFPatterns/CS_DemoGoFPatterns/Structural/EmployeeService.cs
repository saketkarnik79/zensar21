using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public void AddEmployee(string name)
        {
            _employeeRepository.Add(new Employee() { Id = new Random().Next(), Name = name });
        }

        public void PrintAll()
        {
            var employees = _employeeRepository.GetAll();
            foreach (var emp in employees)
            {
                Console.WriteLine($"Employee ID: {emp.Id}, Name: {emp.Name}");
            }
        }
    }
}
