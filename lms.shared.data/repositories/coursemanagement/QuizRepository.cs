using lms.shared.data.entities.coursemanagement.Content;

namespace lms.shared.data.repositories.coursemanagement
{
    public class QuizRepository : IQuizRepository
    {
        public Task<Quiz> AddAsync(Quiz quiz)
        {
            throw new NotImplementedException();
        }

        public Task<Option> AddOptionAsync(Guid questionId, Option option)
        {
            throw new NotImplementedException();
        }

        public Task<Question> AddQuestionAsync(Guid quizId, Question question)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Quiz quiz)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOptionAsync(Option option)
        {
            throw new NotImplementedException();
        }

        public Task DeleteQuestionAsync(Question question)
        {
            throw new NotImplementedException();
        }

        public Task<Quiz> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Quiz> UpdateAsync(Quiz quiz)
        {
            throw new NotImplementedException();
        }

        public Task<Option> UpdateOptionAsync(Option option)
        {
            throw new NotImplementedException();
        }

        public Task<Question> UpdateQuestionAsync(Question question)
        {
            throw new NotImplementedException();
        }
    }
}
