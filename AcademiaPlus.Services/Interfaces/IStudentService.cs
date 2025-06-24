using AcademiaPlus.Models;

namespace AcademiaPlus.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Result> CreateAsync(Student student);
        Task<Result> RegisterSubjectAsync(int studentId, int subjectId);
        Task<Result> UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<Result> UnsubscribeAsync(int studentId, int subjectId);
        bool Exists(int id);
    }
}
