using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PG_DataAccess.Data;
using System;

namespace PG_Tests
{
    class DbSetup : IDisposable
    {
        #region IDisposable Support  
        private bool disposedValue = false; // To detect redundant calls  

        public PgDbContext CreateContextForSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var option = new DbContextOptionsBuilder<PgDbContext>().UseSqlite(connection).Options;

            var context = new PgDbContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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