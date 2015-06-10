
using System;

namespace InstantMessenger.Core.Base
{
    /// <summary>
    /// Represents a transactional job.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        void Rollback();
    }
}
