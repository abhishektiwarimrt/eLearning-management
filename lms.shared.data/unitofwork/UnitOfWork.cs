using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.unitofwork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private bool _disposed = false;

        public TContext Context { get; }

        public UnitOfWork(TContext context)
        {
            Context = context;
        }

        public async Task BeginTransactionAsync()
        {
            await Context.Database.BeginTransactionAsync();
        }


        public async Task<int> CommitAsync()
        {
            try
            {
                var count = await SaveChangesAsync();
                await Context.Database.CommitTransactionAsync();

                return count;
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            await Context.Database.RollbackTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
