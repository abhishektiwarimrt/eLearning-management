using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lms.shared.data.entities.coursemanagement.Content;

namespace lms.services.coursemanagement.Services
{
    // Plan (pseudocode as comments):
    // - Validate course and section exist.
    // - Map incoming DTOs to domain modules once.
    // - Begin transaction, persist modules, commit fast so create is quick.
    // - Map created modules to DTOs.
    // - In a single indexed loop:
    //     - For each input DTO with a File:
    //         - Read file into a byte[] (MemoryStream).
    //         - Create a FileUploadQueueItem referencing the created module id.
    //         - Mark the corresponding result DTO's FileUploaded = true here (avoid a separate pass).
    // - If there are queue items:
    //     - Try to persist them and commit; on failure log and swallow (do not block creation).
    // - Use minimal allocations and a single loop to set flags to improve performance.
    // - Rollback transaction on any outer exception.
    public class CourseModuleService(
        ICourseRepository courseRepository,
        ICourseModuleRepository courseModuleRepository,
        IFileUploadQueueItemRepository fileUploadQueueItemRepository,
        IUnitOfWork<CourseDbContext> unitOfWork)
        : ICourseModuleService
    {        
        public async Task<IList<CourseModuleDto>> CreateModuleAsync(Guid CourseId, Guid CourseSectionId, IList<CourseModuleDto> CourseModules)
        {
            var course = await courseRepository.GetByIdAsync(CourseId) ?? throw new NotFoundException($"Invalid CourseId[{CourseId}]");
            if (!course.Sections.Any(s => s.Id == CourseSectionId))
                throw new NotFoundException($"CourseSectionId[{CourseSectionId}] not found");

            var modules = CourseModules.Adapt<IList<CourseModule>>();

            try
            {
                await unitOfWork.BeginTransactionAsync();
                var created = await courseModuleRepository.AddAsync(CourseSectionId, modules);
                await unitOfWork.CommitAsync();

                var resultDto = created.Adapt<IList<CourseModuleDto>>();

                var queueItems = new List<FileUploadQueueItem>(CourseModules.Count);
                var fileResultIndices = new List<int>(CourseModules.Count);

                for (int i = 0; i < CourseModules.Count; i++)
                {
                    var dto = CourseModules[i];
                    if (dto?.File is null)
                        continue;

                    // Read file bytes once
                    using var ms = new MemoryStream();
                    await dto.File.CopyToAsync(ms);
                    var bytes = ms.ToArray();

                    queueItems.Add(new FileUploadQueueItem
                    {
                        CourseModuleId = created[i].Id,
                        FileName = dto.File.FileName,
                        FileBytes = bytes
                    });

                    // Record which result DTO should be marked if enqueue succeeds.
                    fileResultIndices.Add(i);
                }

                if (queueItems.Count > 0)
                {
                    try
                    {
                        await unitOfWork.BeginTransactionAsync();
                        await fileUploadQueueItemRepository.AddAsync(queueItems);
                        await unitOfWork.CommitAsync();

                        // Only mark FileUploaded = true after successful enqueue+commit
                        for (int j = 0; j < fileResultIndices.Count; j++)
                        {
                            var idx = fileResultIndices[j];
                            if (idx >= 0 && idx < resultDto.Count)
                                resultDto[idx].FileUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log and swallow: file enqueue failure shouldn't block module creation
                        Console.WriteLine($"Error enqueueing file uploads: {ex.Message}");
                        for (int j = 0; j < fileResultIndices.Count; j++)
                        {
                            var idx = fileResultIndices[j];
                            if (idx >= 0 && idx < resultDto.Count)
                                resultDto[idx].FileUploaded = false;
                        }
                        // Ensure FileUploaded flags remain false (they were never set)
                    }
                }

                return resultDto;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public Task DeleteModuleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CourseModuleDto> GetModuleByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseModuleDto>> GetModulesByCourseIdAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<CourseModuleDto> UpdateModuleAsync(CourseModuleDto moduleDto)
        {
            throw new NotImplementedException();
        }

        // other methods...
    }
}


