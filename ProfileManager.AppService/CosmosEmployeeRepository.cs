using ProfileManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    public class DocumentEmployeeRepository : IEmployeeRepository
    {
        private IDocumentProvider<Employee> _repo;

        public DocumentEmployeeRepository(IDocumentProvider<Employee> repo)
        {
            _repo = repo;
        }

        public async Task<Employee> GetEmployeeAsync(string id)
        {
            return await _repo.GetDocumentAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate)
        {
            return await _repo.GetDocumentsAsync(predicate);
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee e)
        {
            return await _repo.ReplaceDocumentAsync(e.Id, e);
        }

        public async Task DeleteEmployeeAsync(Employee e)
        {
            await _repo.DeleteDocumentAsync(e.Id);
        }
    }
}
