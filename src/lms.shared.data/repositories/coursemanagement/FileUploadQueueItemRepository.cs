using lms.shared.data.dbcontexts;
using lms.shared.data.entities.coursemanagement.Content;
using Microsoft.EntityFrameworkCore;

namespace lms.shared.data.repositories.coursemanagement
{
    public class FileUploadQueueItemRepository(CourseDbContext context) : IFileUploadQueueItemRepository
    {
        public async Task<IList<FileUploadQueueItem>> AddAsync(IList<FileUploadQueueItem> fileUploadQueueItems)
        {
            var creationTime = DateTime.UtcNow;
            foreach (var fileUploadQueueItem in fileUploadQueueItems)
            {
                fileUploadQueueItem.Id = Guid.NewGuid();
                fileUploadQueueItem.QueuedAt = creationTime;
            }
            await context.FileUploadQueueItems.AddRangeAsync(fileUploadQueueItems);
            return fileUploadQueueItems;
        }

        public Task DeleteAsync(FileUploadQueueItem fileUploadQueueItem)
        {
            context.FileUploadQueueItems.Remove(fileUploadQueueItem);
            // Do not call SaveChanges here; UnitOfWork controls transactions / saves.
            return Task.CompletedTask;
        }

        public async Task<FileUploadQueueItem> GetByIdAsync(Guid id)
        {
            return await context.FileUploadQueueItems
                .Include(f => f.CourseModule)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<FileUploadQueueItem>> GetPendingItemsAsync(int limit)
        {
            return await context.FileUploadQueueItems              
                .Where(f => f.QueueStatus == "Pending")
                .OrderBy(f => f.QueuedAt)
                .Take(limit)
                .ToListAsync();
        }

        public Task<FileUploadQueueItem> UpdateAsync(FileUploadQueueItem fileUploadQueueItem)
        {
            // Mark entity as updated in the DbContext. Do NOT call SaveChanges here:
            // UnitOfWork is expected to control Save/Commit/Transaction.
            context.FileUploadQueueItems.Update(fileUploadQueueItem);
            return Task.FromResult(fileUploadQueueItem);
        }
    }
}