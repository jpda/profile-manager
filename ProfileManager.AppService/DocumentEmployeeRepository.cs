using ProfileManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    // todo: handle the success == false case with the change to serviceresult
    public class DocumentEmployeeRepository : IEmployeeRepository
    {
        private IDocumentProvider<Employee> _repo;

        public DocumentEmployeeRepository(IDocumentProvider<Employee> repo)
        {
            _repo = repo;
        }

        public async Task<Employee> GetEmployeeAsync(string immutableId)
        {

            var result = await _repo.GetDocumentAsync(immutableId);
            return result.Value;
        }

        //todo: handle
        public async Task<Employee> GetEmployeeByEmployeeIdAsync(string id)
        {
            var result = await _repo.GetDocumentsAsync(x => x.CompanyId == id);

            var results = result.Value;

            if (!results.Any()) return new Employee();
            if (results.Count() > 1)
            {
                throw new Exception("too many employees match that id");
            }
            return results.Single();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Expression<Func<Employee, bool>> predicate)
        {
            return (await _repo.GetDocumentsAsync(predicate)).Value;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return (await _repo.GetDocumentsAsync(x => true)).Value;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee e)
        {
            return (await _repo.ReplaceDocumentAsync(e.ImmutableId, e)).Value;
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
            // since cosmos' auto-generated IDs are all guids, need to come up with a shorthand here that's easy for humans to use
            // todo: figure out something better than Random - sticking to 4 digits here for readability but obviously this would need to change 
            // todo: at this point, it might be cheaper to just ping the DB with the ID to check for a result rather than wait for and catch an exception, although not found throws an exception too
            var r = new Random();
            var empId = r.Next(0, 9999).ToString("D4");
            e.CompanyId = empId;
            try
            {
                var employeeRecord = await _repo.CreateDocumentAsync(e);
                if (!employeeRecord.Success && employeeRecord.ErrorCode == "Conflict")
                {
                    await CreateEmployeeAsync(e);
                }
                //todo : handle other error cases better than rethrowing
                else if (!employeeRecord.Success && employeeRecord.Exception != null)
                {
                    throw employeeRecord.Exception;
                }
                else
                {
                    throw new Exception(employeeRecord.Message);
                }
                return employeeRecord.Value;
            }
            catch (Exception ex)
            {
                // todo: needs exception handling and retry for all these network calls
                Console.WriteLine(ex);
            }
            // todo: hate to return null
            return null;
        }
    }
}
