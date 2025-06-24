using AcademiaPlus.Data;
using AcademiaPlus.Models;
using AcademiaPlus.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AcademiaPlus.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly AcademiaContext _context;

        public RegistrationService(AcademiaContext context)
        {
            _context = context;
        }

        public async Task<List<Registration>> GetAllAsync() =>
            await _context.Registration.Include(r => r.Student).Include(r => r.Subject).ToListAsync();

        public async Task<Result> RegisterSubjectAsync(int studentId, int subjectId)
        {
            var student = await _context.Student
                .Include(s => s.Registrations)
                .ThenInclude(r => r.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return Result.Failure("Estudiante no encontrado.");

            var heavySubjects = student.Registrations.Count(r => r.Subject.Credits > 4);
            var newSubject = await _context.Subject.FindAsync(subjectId);

            if (newSubject.Credits > 4 && heavySubjects >= 3)
                return Result.Failure("No puede inscribir más de 3 materias con más de 4 créditos.");

            _context.Registration.Add(new Registration { StudentId = studentId, SubjectId = subjectId });
            await _context.SaveChangesAsync();

            return Result.Success();
        }


        public async Task UnregisterSubjectAsync(int studentId, int subjectId)
        {
            var registration = await _context.Registration
                .FirstOrDefaultAsync(r => r.StudentId == studentId && r.SubjectId == subjectId);
            if (registration != null)
            {
                _context.Registration.Remove(registration);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CanRegisterSubjectAsync(int studentId, int subjectId)
        {
            var subject = await _context.Subject.FindAsync(subjectId);
            if (subject == null)
                return false;

            var registeredSubjects = await _context.Registration
                .Include(r => r.Subject)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();

            int heavySubjectCount = registeredSubjects.Count(s => s.Subject.Credits > 4);
            if (subject.Credits > 4 && heavySubjectCount >= 3)
                return false;

            return true;
        }
    }
}
