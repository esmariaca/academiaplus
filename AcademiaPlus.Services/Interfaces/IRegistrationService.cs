using AcademiaPlus.Models;

namespace AcademiaPlus.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<List<Registration>> GetAllAsync();
        Task<Result> RegisterSubjectAsync(int studentId, int subjectId);
        Task UnregisterSubjectAsync(int studentId, int subjectId);
        Task<bool> CanRegisterSubjectAsync(int studentId, int subjectId);
    }
}
