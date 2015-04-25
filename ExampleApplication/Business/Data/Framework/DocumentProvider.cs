using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace ExampleApplication.Business.Data.Framework
{
    public interface IDocumentProvider
    {
        IEnumerable<T> Query<T>();
        IEnumerable<T> Query<T>(Func<T, bool> query);
        Task DestroyDatabaseAsync<T>();
        Task CreateDocumentAsync<T>(T newDocument);
        Task UpdateDocumentAsync(Document document);
    }

    public class DocumentProvider : IDocumentProvider
    {
        private readonly DocumentClient _client;
        private readonly object _collectionReadSync = new object();

        private readonly Dictionary<string, DocumentCollection> _collections =
            new Dictionary<string, DocumentCollection>();

        private readonly object _databaseReadSync = new object();
        private readonly Dictionary<string, Database> _databases = new Dictionary<string, Database>();

        public DocumentProvider(DocumentClient client)
        {
            _client = client;
        }

        private Database GetOrCreateDatabase(string dbName)
        {
            lock (_databaseReadSync)
            {
                if (_databases.ContainsKey(dbName))
                {
                    return _databases[dbName];
                }
                var db = _client.CreateDatabaseQuery()
                    .Where(d => d.Id == dbName)
                    .AsEnumerable()
                    .SingleOrDefault() ?? _client.CreateDatabaseAsync(new Database {Id = dbName}).Result;

                _databases.Add(dbName, db);
                return db;
            }
        }

        private DocumentCollection GetOrCreateCollGetCollection(Database database, string collectionName)
        {
            lock (_collectionReadSync)
            {
                if (_collections.ContainsKey(collectionName))
                {
                    return _collections[collectionName];
                }
                var col = _client.CreateDocumentCollectionQuery(database.CollectionsLink)
                    .Where(c => c.Id == collectionName)
                    .AsEnumerable()
                    .FirstOrDefault() ??
                                         _client.CreateDocumentCollectionAsync(database.SelfLink,
                                             new DocumentCollection {Id = collectionName}).Result;

                _collections.Add(collectionName, col);
                return col;
            }
        }

        public DocumentCollection GetOrCreateCollGetCollection<T>()
        {
            var attribute = typeof (T).GetCustomAttribute<DocumentAttribute>();
            return attribute == null
                ? GetOrCreateCollGetCollection(GetOrCreateDatabase(DocumentAttribute.DefaultDatabase), DocumentAttribute.DefaultCollection)
                : GetOrCreateCollGetCollection(GetOrCreateDatabase(attribute.DatabaseName), attribute.CollectionName);
        }

        public IEnumerable<T> Query<T>()
        {
            return Query<T>(null);
        }

        public IEnumerable<T> Query<T>(Func<T, bool> query)
        {
            var collection = GetOrCreateCollGetCollection<T>();
            var queryable = _client.CreateDocumentQuery<T>(collection.DocumentsLink);
            return query == null
                ? queryable.AsEnumerable()
                : queryable.Where(query).AsEnumerable();
        }

        public async Task DestroyDatabaseAsync<T>()
        {
            var attribute = typeof(T).GetCustomAttribute<DocumentAttribute>();
            var database = attribute == null
                ? GetOrCreateDatabase(DocumentAttribute.DefaultDatabase)
                : GetOrCreateDatabase(attribute.DatabaseName);

            // I know it's kinda lazy, coz I may have just created the database I'm about to delete
            await _client.DeleteDatabaseAsync(database.SelfLink);

            // not perfect, but forces us to re-cache these.
            _databases.Clear();
            _collections.Clear();
        }

        public async Task CreateDocumentAsync<T>(T newDocument)
        {
            var collection = GetOrCreateCollGetCollection<T>();
            await _client.CreateDocumentAsync(collection.SelfLink, newDocument);
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            await _client.ReplaceDocumentAsync(document);
        }
    }
}