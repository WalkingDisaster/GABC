using System;

namespace ExampleApplication.Business.Data.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DocumentAttribute : Attribute
    {
        public const string DefaultDatabase = "DefaultDb";
        public const string DefaultCollection = "DefaultCollection";

        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public DocumentAttribute(string databaseName, string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        public DocumentAttribute(string databaseName, Type collectionType)
        {
            DatabaseName = databaseName;
            CollectionName = collectionType.Name + "Collection";
        }

        public DocumentAttribute()
        {
            DatabaseName = DefaultDatabase;
            CollectionName = DefaultCollection;
        }
    }
}