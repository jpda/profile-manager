using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace ProfileManager.AppService
{
    public interface IDocumentProvider<T> where T : class
    {
        Task<T> CreateDocumentAsync(T item);
        Task<T> ReplaceDocumentAsync(string id, T item);
        Task DeleteDocumentAsync(string id);
        Task<IEnumerable<T>> GetDocumentsAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetDocumentAsync(string id);
    }
}