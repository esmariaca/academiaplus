using AcademiaPlus.Models;

namespace AcademiaPlus.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<Subject>> GetAllAsync();
    }
}
