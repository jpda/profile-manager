using ProfileManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Employee> GetEmployeeAsync(string immutableId)
        {
            return await _repo.GetDocumentAsync(immutableId);
        }

        //todo: clean this up
        public async Task<Employee> GetEmployeeByEmployeeIdAsync(string id)
        {
            var results = await _repo.GetDocumentsAsync(x => x.CompanyId == id);
            if (!results.Any()) return new Employee();
            if (results.Count() > 1)
            {
                throw new Exception("too many employees match that id");
            }
            return results.Single();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate)
        {
            return await _repo.GetDocumentsAsync(predicate);
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _repo.GetDocumentsAsync(x => true);
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee e)
        {
            return await _repo.ReplaceDocumentAsync(e.ImmutableId, e);
        }

        public async Task DeleteEmployeeAsync(Employee e)
        {
            await _repo.DeleteDocumentAsync(e.ImmutableId);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee e)
        {
            // enforce the new immutable ID here, so if we expand to other consumers later the rule is still enforced
            if (string.IsNullOrEmpty(e.ImmutableId))
            {
                e.ImmutableId = Guid.NewGuid().ToString();
            }
            return await _repo.CreateDocumentAsync(e);
        }
    }
}
