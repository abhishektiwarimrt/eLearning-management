using lms.services.aws.S3;

namespace lms.services.coursemanagement.Services
{
    public class FileUploadWorkerService (IServiceProvider _services, ILogger<FileUploadWorkerService> _logger) : BackgroundService
    {        
        private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessPendingUploadsAsync(stoppingToken);
                await Task.Delay(_pollInterval, stoppingToken);
            }
        }

        private async Task ProcessPendingUploadsAsync(CancellationToken ct)
        {
            using var scope = _services.CreateScope();
            var courseModuleRepo = scope.ServiceProvider.GetRequiredService<ICourseModuleRepository>();
            var repo = scope.ServiceProvider.GetRequiredService<IFileUploadQueueItemRepository>();
            var s3 = scope.ServiceProvider.GetRequiredService<IS3ServiceEvent>();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork<CourseDbContext>>();


            // In ProcessPendingUploadsAsync:
            var pending = await repo.GetPendingItemsAsync(limit: 10);

            if(!pending.Any())
            {
                return;
            }

            
            foreach (var queueItem in pending)
            {
                if (ct.IsCancellationRequested)
                    break;

                try
                {
                    await uow.BeginTransactionAsync();
                    queueItem.QueueStatus = "Processing";
                    queueItem.RetryCount++;
                    await repo.UpdateAsync(queueItem);
                    await uow.CommitAsync();

                    // Transaction #2: persist module changes and mark queue item completed
                    await uow.BeginTransactionAsync();
                    if (queueItem.FileBytes != null)
                    {
                        using var ms = new MemoryStream(queueItem.FileBytes);
                        //var key = await s3.UploadFileAsync(ms, queueItem.FileName);
                        var key = "course-modules/" + Guid.NewGuid() + "-" + queueItem.FileName;
                        // Patch CourseModule
                        var module = await courseModuleRepo.GetByIdAsync(queueItem.CourseModuleId);
                        if (module != null)
                        {
                            module.ContentReference = key;
                            module.Uploaded = true;
                            module.UploadedAt = DateTime.UtcNow;
                            await courseModuleRepo.UpdateAsync(module);
                        }
                    }
                    
                    queueItem.QueueStatus = "Completed";
                    queueItem.ProcessedAt = DateTime.UtcNow;
                    queueItem.FileBytes = null;
                    await repo.UpdateAsync(queueItem);
                    await uow.CommitAsync();
                }
                catch
                {
                    try
                    {
                        await uow.RollbackAsync();
                    }
                    catch { }
                    _logger.LogError("Error processing file upload queue item {QueueItemId}", queueItem.Id);
                }
            }

        }
    }
}