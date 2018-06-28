using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    public class CosmosDocumentProvider<T> : IDocumentProvider<T> where T : class
    {
        private DocumentClient _client;
        private readonly string _databaseId;
        private readonly string _collectionId;

        //todo: don't like IOptions permeating everything
        //todo: push IOptions further up to the callers instead of the service itself
        private readonly IOptions<DocumentProviderOptions> _options;

        // keeping these ctors to limit the blast radius when inevitably i yank out IOptions since it is continuing to bug me that it is in here
        public CosmosDocumentProvider(IOptions<DocumentProviderOptions> options) : this(new DocumentClient(new Uri(options.Value.Endpoint), options.Value.Key), options) { }
        public CosmosDocumentProvider(DocumentClient client, IOptions<DocumentProviderOptions> options) : this(new DocumentClient(new Uri(options.Value.Endpoint), options.Value.Key), options.Value.Database, options.Value.Collection)
        {
            _options = options;
        }
        public CosmosDocumentProvider(DocumentClient client, string db, string collection)
        {
            _client = client;
            _databaseId = db;
            _collectionId = collection;
        }

        public async Task<ServiceResult<T>> GetDocumentAsync(string id)
        {
            try
            {
                var document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return new ServiceResult<T>((T)(dynamic)document.Resource);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // todo: be better
                    return null;
                }
                else
                {
                    //todo: do something here
                    throw;
                }
            }
        }

        public async Task<ServiceResult<IEnumerable<T>>> GetDocumentsAsync(Expression<Func<T, bool>> predicate)
        {
            List<T> results = new List<T>();
            try
            {
                var query = _client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), new FeedOptions { MaxItemCount = -1 }).Where(predicate).AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
            }
            catch (DocumentClientException notFound)
            {
                // todo: log this
                return new ServiceResult<IEnumerable<T>>() { ErrorCode = "NotFound", Success = false };
            }
            catch (Exception ex)
            {
                throw;
            }
            return new ServiceResult<IEnumerable<T>>(results);
        }

        //todo: consider returning Document for additional metadata? too leaky since it's implementation specific?
        // if IDs collide, Cosmos will throw an exception
        public async Task<ServiceResult<T>> CreateDocumentAsync(T item)
        {
            try
            {
                var response = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
                return new ServiceResult<T>((T)(dynamic)response.Resource);
            }
            catch (DocumentClientException ex)
            {
                return new ServiceResult<T>() { Success = false, Message = ex.Message, ErrorCode = ex.Error.Code, Exception = ex };
            }
        }

        public async Task<ServiceResult<T>> ReplaceDocumentAsync(string id, T item)
        {
            var response = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
            return new ServiceResult<T>((T)(dynamic)response.Resource);
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }
    }
}
