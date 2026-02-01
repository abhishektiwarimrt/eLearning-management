using lms.shared.data.entities.coursemanagement.Content;

namespace lms.shared.data.repositories.coursemanagement
{
    public interface IQuizRepository
    {
        Task<Quiz> GetByIdAsync(Guid id);
        Task<Quiz> AddAsync(Quiz quiz);
        Task<Quiz> UpdateAsync(Quiz quiz);
        Task DeleteAsync(Quiz quiz);
        Task<Question> AddQuestionAsync(Guid quizId, Question question);
        Task<Question> UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(Question question);
        Task<Option> AddOptionAsync(Guid questionId, Option option);
        Task<Option> UpdateOptionAsync(Option option);
        Task DeleteOptionAsync(Option option);
    }
}
