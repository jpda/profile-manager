﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProfileManager.AppService
{
    public class CosmosDocumentRepo<T> : IDocumentRepository<T> where T : class
    {
        private DocumentClient _client;
        private string _databaseId;
        private string _collectionId;
        public CosmosDocumentRepo(string endpoint, string key, string db, string collection) : this(new DocumentClient(new Uri(endpoint), key), db, collection) { }
        public CosmosDocumentRepo(DocumentClient client, string db, string collection)
        {
            _client = client;
            _databaseId = db;
            _collectionId = collection;
        }

        public async Task<T> GetDocumentAsync(string id)
        {
            try
            {
                var document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return (T)(dynamic)document.Resource;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    //todo: do something here
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetDocumentsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), new FeedOptions { MaxItemCount = -1 }).Where(predicate).AsDocumentQuery();
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }

        //todo: consider returning Document for additional metadata? or too leaky?
        public async Task<T> CreateDocumentAsync(T item)
        {
            var response = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
            return (T)(dynamic)response.Resource;
        }

        public async Task<T> ReplaceDocumentAsync(string id, T item)
        {
            var response = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
            return (T)(dynamic)response.Resource;
        }

        public async Task DeleteDocumentAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }
    }
}
