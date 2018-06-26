using ProfileManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeAsync(string immutableId);
        Task<Employee> GetEmployeeByEmployeeIdAsync(string id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate);
        Task<Employee> CreateEmployeeAsync(Employee e);
        Task<Employee> UpdateEmployeeAsync(Employee e);
        Task DeleteEmployeeAsync(Employee e);
    }
}

