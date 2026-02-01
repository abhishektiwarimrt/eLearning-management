using lms.shared.data.entities.coursemanagement;
using lms.shared.data.entities.coursemanagement.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.shared.data.repositories.coursemanagement
{
    public interface IFileUploadQueueItemRepository
    {
        Task<FileUploadQueueItem> GetByIdAsync(Guid id);
        Task<IList<FileUploadQueueItem>> AddAsync(IList<FileUploadQueueItem> fileUploadQueueItems);
        Task<FileUploadQueueItem> UpdateAsync(FileUploadQueueItem fileUploadQueueItem);
        Task DeleteAsync(FileUploadQueueItem fileUploadQueueItem);
        Task<IEnumerable<FileUploadQueueItem>> GetPendingItemsAsync(int limit);
    }
}
