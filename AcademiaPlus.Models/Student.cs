using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.Models
{
    [ExcludeFromCodeCoverage]
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Document { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public List<Registration> Registrations { get; set; } = new List<Registration>();
    }
}

