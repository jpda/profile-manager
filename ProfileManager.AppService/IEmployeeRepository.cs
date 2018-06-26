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
        Task<Employee> GetEmployeeAsync(string id);
        Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate);
        Task<Employee> UpdateEmployeeAsync(Employee e);
        Task DeleteEmployeeAsync(Employee e);
    }
}
