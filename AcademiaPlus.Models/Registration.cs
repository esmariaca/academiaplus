using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.Models
{
    [ExcludeFromCodeCoverage]
    public class Registration
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

    }
}
