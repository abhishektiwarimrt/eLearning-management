namespace lms.shared.data.entities.coursemanagement.Content
{
    public class Question
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public List<Option> Options { get; set; } = [];
    }
}