using lms.shared.data.entities.instructormanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lms.shared.data.repositories.instructormanagement
{
    public class InstructorRepository : IInstructorRepository
    {
        public Task<bool> AddProfile(InstructorProfile profile)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProfile(InstructorProfile profile)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProfile(InstructorProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}
