using AcademiaPlus.Data;
using AcademiaPlus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace AcademiaPlus.Services.Test
{
    public class StudentServiceTests
    {
        private AcademiaContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AcademiaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AcademiaContext(options);
        }
        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_IfDuplicateDocument()
        {
            var logger = NullLogger<StudentService>.Instance;
            var context = GetInMemoryContext();
            context.Student.Add(new Student { Id = 1, Name = "John", Lastname = "Morales", Document = "12546389", Email = "el_esJhob@gmail.com" });
            await context.SaveChangesAsync();

            var service = new StudentService(context, logger);
            var result = await service.CreateAsync(new Student { Id = 2, Name = "John", Lastname = "Morales", Document = "12546389", Email = "duplicate@gmail.com" });

            Assert.False(result.IsSuccess);
            Assert.Equal("Ya existe un estudiante con este documento.", result.ErrorMessage);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddStudent_IfNewDocument()
        {
            var logger = NullLogger<StudentService>.Instance;
            var context = GetInMemoryContext();
            var service = new StudentService(context, logger);

            var result = await service.CreateAsync(new Student { Id = 2, Name = "Maria", Lastname = "Villa", Document = "6548455", Email = "villaes@gmail.com" });

            Assert.True(result.IsSuccess);
            Assert.Equal(1, await context.Student.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFailure_IfStudentNotFound()
        {
            var logger = NullLogger<StudentService>.Instance;
            var context = GetInMemoryContext();
            var service = new StudentService(context, logger);

            var result = await service.UpdateAsync(new Student { Id = 2, Name = "MariaAle", Lastname = "Perez", Document = "56165415", Email = "maria@example.com" });

            Assert.False(result.IsSuccess);
            Assert.Equal("Estudiante no encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateStudent_IfExists()
        {
            var logger = NullLogger<StudentService>.Instance;
            var context = GetInMemoryContext();
            context.Student.Add(new Student { Id = 2, Name = "MariaAle", Lastname = "Perez", Document = "56165415", Email = "maria@example.com" });
            await context.SaveChangesAsync();

            var service = new StudentService(context, logger);
            var result = await service.UpdateAsync(new Student { Id = 2, Name = "Johnny", Lastname = "Perez", Document = "56165415", Email = "maria@example.com" });

            Assert.True(result.IsSuccess);
            var updated = await context.Student.FindAsync(2);
            Assert.Equal("Johnny", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveStudent_IfExists()
        {
            var logger = NullLogger<StudentService>.Instance;
            var context = GetInMemoryContext();
            context.Student.Add(new Student { Id = 1, Name = "John", Lastname = "Morales", Document = "12546389", Email = "john@gmail.com" });
            await context.SaveChangesAsync();

            var service = new StudentService(context, logger);
            await service.DeleteAsync(1);

            Assert.Equal(0, await context.Student.CountAsync());
        }
    }
}