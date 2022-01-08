// Kikitori
// (C) 2021, Andreas Gaiser

using System;
using System.Collections.Generic;
using SQLite;

namespace Kikitori.Data
{

    public class Database : IDisposable
    {
        private static Database instance;
        public static Database Instance
        {
            get
            {
                return instance ?? (instance = new Database());
            }
        }

        private SQLiteConnection database;

        private void CreateTables()
        {
            database.CreateTable<SentenceItem>();
            database.CreateTable<Medium>();
        }

        private string path;
        public string Path
        {
            get => path ?? "<not loaded>";
        }

        public void LoadOrCreateDatabase(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            path = databasePath;
            CreateTables();
        }

        public List<T> GetItems<T>()
            where T : Entity, new()
        {
            return database.Table<T>().ToList();
        }

        public int Save<T>(T item)
            where T : Entity
        {
            return database.Insert(item);
        }

        public void Update<T>(T item)
            where T : Entity
        {
            database.Update(item);
        }

        public void Delete<T>(T item)
            where T : Entity
        {
            database.Delete(item);
        }

        public void SaveAndClose()
        {
            database.Close();
            database = new SQLiteConnection(Path);
        }

        public void ClearAll<T>()
        {
            Console.WriteLine("Deleted " + database.DeleteAll<T>() + " entries.");
        }

        public bool IsLoaded()
        {
            return database != null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    database.Close();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
