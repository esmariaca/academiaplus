using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.Models
{
    [ExcludeFromCodeCoverage]
    //Materia
    public class Subject
    {
        public int Id { get; set; }

        public string Matter { get; set; }

        public string Code { get; set; }

        public int Credits { get; set; }

        public List<Registration> Registrations { get; set; } = new();
    }
}
