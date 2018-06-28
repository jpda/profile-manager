using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace ProfileManager.AppService
{
    public interface IDocumentProvider<T> where T : class
    {
        Task<ServiceResult<T>> CreateDocumentAsync(T item);
        Task<ServiceResult<T>> ReplaceDocumentAsync(string id, T item);
        Task DeleteDocumentAsync(string id);
        Task<ServiceResult<IEnumerable<T>>> GetDocumentsAsync(Expression<Func<T, bool>> predicate);
        Task<ServiceResult<T>> GetDocumentAsync(string id);
    }
}