
using FjordNestPro.Models;

namespace FjordNestPro.Data
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetQuestionsAsync();
        Task<Question> GetQuestionByIdAsync(int id);
        Task<Question> GetQuestionByIdAsync(int? id);
        Task AddAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Question question);
        Task SaveChangesAsync();
        bool QuestionExists(int id);
        // Add other necessary methods if required.
    }
}
