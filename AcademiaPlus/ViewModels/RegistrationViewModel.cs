using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class RegistrationViewModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int SubjectId { get; set; }

        public StudentViewModel? Student { get; set; }
        public SubjectViewModel? Subject { get; set; }

        public List<StudentViewModel> Students { get; set; } = new();
        public List<SubjectViewModel> Subjects { get; set; } = new();
    }
}
