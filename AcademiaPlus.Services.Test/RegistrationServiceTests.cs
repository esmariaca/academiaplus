using AcademiaPlus.Data;
using AcademiaPlus.Models;
using AcademiaPlus.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AcademiaPlus.Services.Test
{
    public class RegistrationServiceTests
    {
        private AcademiaContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AcademiaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AcademiaContext(options);
        }

        [Fact]
        public async Task RegisterSubjectAsync_ShouldFail_IfStudentNotFound()
        {
            var context = GetInMemoryContext();
            IRegistrationService service = new RegistrationService(context);

            var result = await service.RegisterSubjectAsync(99, 1);

            Assert.False(result.IsSuccess);
            Assert.Equal("Estudiante no encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task RegisterSubjectAsync_ShouldFail_IfHeavyLimitExceeded()
        {
            var context = GetInMemoryContext();

            var student = new Student { Id = 1, Name = "John", Lastname = "Morales", Document = "12546389", Email = "el_esJhob@gmail.com" };
            var heavySubject1 = new Subject { Id = 1, Matter = "Matematicas", Code = "1236", Credits = 5 };
            var heavySubject2 = new Subject { Id = 2, Matter = "Fisica", Code = "7896", Credits = 5 };
            var newHeavySubject = new Subject { Id = 3, Matter = "Quimica", Code = "4567", Credits = 5 };

            context.Student.Add(student);
            context.Subject.AddRange(heavySubject1, heavySubject2, newHeavySubject);

            context.Registration.AddRange(
                new Registration { StudentId = 1, SubjectId = 1 },
                new Registration { StudentId = 1, SubjectId = 2 },
                new Registration { StudentId = 1, SubjectId = 3 } 
            );

            await context.SaveChangesAsync();

            var service = new RegistrationService(context);
            var result = await service.RegisterSubjectAsync(1, 3);

            Assert.False(result.IsSuccess);
            Assert.Equal("No puede inscribir más de 3 materias con más de 4 créditos.", result.ErrorMessage);
        }

        [Fact]
        public async Task RegisterSubjectAsync_ShouldSucceed_IfWithinLimits()
        {
            var context = GetInMemoryContext();

            var student = new Student
            {
                Id = 1,
                Name = "John",
                Lastname = "Doe",
                Document = "12345678",
                Email = "john.doe@example.com",
                Registrations = new List<Registration>()
            };

            var lightSubject = new Subject { Id = 3, Matter = "Historia", Code = "1236", Credits = 3 };

            context.Student.Add(student);
            context.Subject.Add(lightSubject);

            await context.SaveChangesAsync();

            var service = new RegistrationService(context);
            var result = await service.RegisterSubjectAsync(1, 3);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, await context.Registration.CountAsync());
        }

        [Fact]
        public async Task UnregisterSubjectAsync_ShouldRemoveRegistration_IfExists()
        {
            var context = GetInMemoryContext();

            var reg = new Registration { StudentId = 1, SubjectId = 1 };
            context.Registration.Add(reg);
            await context.SaveChangesAsync();

            var service = new RegistrationService(context);
            await service.UnregisterSubjectAsync(1, 1);

            Assert.Equal(0, await context.Registration.CountAsync());
        }

        [Fact]
        public async Task CanRegisterSubjectAsync_ShouldReturnFalse_IfHeavyLimit()
        {
            var context = GetInMemoryContext();

            var student = new Student { Id = 1, Name = "John" };
            var heavySubject2 = new Subject { Id = 2, Matter = "Fisica", Code = "7896", Credits = 5 };
            var heavySubject3 = new Subject { Id = 3, Matter = "Quimica", Code = "4567", Credits = 5 };
            context.Subject.AddRange(heavySubject2, heavySubject3);

            context.Registration.AddRange(
                new Registration { StudentId = 1, SubjectId = 1 },
                new Registration { StudentId = 1, SubjectId = 2 },
                new Registration { StudentId = 1, SubjectId = 3 }
            );
            await context.SaveChangesAsync();

            var service = new RegistrationService(context);
            var canRegister = await service.CanRegisterSubjectAsync(1, 1);

            Assert.False(canRegister);
        }
    }
}
