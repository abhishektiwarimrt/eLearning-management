namespace lms.shared.data.unitofwork
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        TContext Context { get; }

        /// <summary>
        /// Asynchronously starts a new transaction.
        ///
        /// Exceptions:
        ///   T:System.OperationCanceledException:
        ///     If the System.Threading.CancellationToken is canceled.
        ///
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous transaction initialization. The task
        ///  result contains a Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction
        ///  that represents the started transaction.
        /// </returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Saving the Data & Applies the outstanding operations in the current transaction to the database.
        /// ///IF Fail to commit then discards the outstanding operations in the current transaction.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        Task<int> CommitAsync();

        /// <summary>
        ///  Discards the outstanding operations in the current transaction.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        Task RollbackAsync();

        /// <summary>
        /// Saves all the changes
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        Task<int> SaveChangesAsync();
    }
}
