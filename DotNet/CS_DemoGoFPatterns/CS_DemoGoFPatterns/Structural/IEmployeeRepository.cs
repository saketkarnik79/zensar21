using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAll();
        Employee GetById(int id);
        void Add(Employee employee);
    }
}
