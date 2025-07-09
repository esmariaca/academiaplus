using AcademiaPlus.Data;
using AcademiaPlus.Models;
using AcademiaPlus.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AcademiaPlus.Services
{
    public class StudentService : IStudentService
    {
        private readonly AcademiaContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(AcademiaContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Student
                .Include(s => s.Registrations)
                .ThenInclude(r => r.Subject)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Student.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Student?> GetMatersByIdAsync(int id)
        {
            return await _context.Student
                    .Include(s => s.Registrations)
                    .ThenInclude(r => r.Subject)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Result> CreateAsync(Student student)
        {
            try
            {
                _logger.LogInformation("🔍 Guardando estudiante en base de datos...");

                if (await _context.Student.AnyAsync(s => s.Document == student.Document))
                    return Result.Failure("Ya existe un estudiante con este documento.");

                if (string.IsNullOrWhiteSpace(student.Email) || !student.Email.Contains("@"))
                    return Result.Failure("El correo electrónico no es válido.");

                _context.Student.Add(student);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Estudiante guardado en base de datos.");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar estudiante.");
                return Result.Failure("Ocurrió un error inesperado.");
            }
        }

        public async Task<Result> RegisterSubjectAsync(int studentId, int subjectId)
        {
            try
            {
                Console.WriteLine(studentId);
                var student = await _context.Student
                    .Include(s => s.Registrations)
                    .ThenInclude(r => r.Subject)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                    return Result.Failure("Estudiante no encontrado.");

                var subject = await _context.Subject.FindAsync(subjectId);
                if (subject == null)
                    return Result.Failure("Materia no encontrada.");

                // Validar total de créditos inscritos
                int totalCredits = student.Registrations.Sum(r => r.Subject.Credits);
                if (totalCredits + subject.Credits > 18) 
                    return Result.Failure("No puedes inscribir más de 18 créditos.");

                if (student.Registrations.Any(r => r.SubjectId == subjectId))
                    return Result.Failure("El estudiante ya tiene esta materia inscrita.");

                var registration = new Registration
                {
                    StudentId = studentId,
                    SubjectId = subjectId
                };

                _context.Registration.Add(registration);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al inscribir materia.");
                return Result.Failure("Ocurrió un error inesperado.");
            }
        }
        public async Task<Result> UpdateAsync(Student student)
        {
            var existingStudent = await _context.Student.FindAsync(student.Id);
            if (existingStudent == null)
                return Result.Failure("Estudiante no encontrado.");

            if (!string.IsNullOrWhiteSpace(student.Name))
                existingStudent.Name = student.Name;

            if (!string.IsNullOrWhiteSpace(student.Lastname))
                existingStudent.Lastname = student.Lastname;

            if (!string.IsNullOrWhiteSpace(student.Document))
                existingStudent.Document = student.Document;

            if (!string.IsNullOrWhiteSpace(student.Email))
                existingStudent.Email = student.Email;

            var resul = await _context.SaveChangesAsync();
            Console.WriteLine(resul);
            return Result.Success();
        }
        public async Task DeleteAsync(int id)
        {
            var student = await _context.Student
                                        .Include(s => s.Registrations)
                                        .FirstOrDefaultAsync(s => s.Id == id);

            if (student != null)
            {
                _context.Registration.RemoveRange(student.Registrations);

                _context.Student.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Result> UnsubscribeAsync(int studentId, int subjectId)
        {
            var registration = await _context.Registration
        .FirstOrDefaultAsync(r => r.StudentId == studentId && r.SubjectId == subjectId);

            if (registration == null)
                return Result.Failure("Inscripción no encontrada.");

            _context.Registration.Remove(registration);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        public bool Exists(int id) => _context.Student.Any(s => s.Id == id);
    }
}
