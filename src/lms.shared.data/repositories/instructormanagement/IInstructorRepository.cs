using lms.shared.data.entities.instructormanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.shared.data.repositories.instructormanagement
{
    public interface IInstructorRepository
    {
        Task<bool> AddProfile(InstructorProfile profile);
        Task<bool> UpdateProfile(InstructorProfile profile);
        Task<bool> DeleteProfile(InstructorProfile profile);
    }
}
