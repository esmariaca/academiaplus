using AcademiaPlus.Data;
using AcademiaPlus.Models;
using AcademiaPlus.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AcademiaPlus.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly AcademiaContext _context;
        private readonly ILogger<StudentService> _logger;

        public SubjectService(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllAsync()
        {
            try
            {
                 return await _context.Subject.ToListAsync();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "❌ Error al obtener materias.");
                Console.WriteLine($"❌ Error al obtener materias: {ex.Message}");
                return new List<Subject>();
            }
        }

    }
}
