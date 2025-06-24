using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string? Name { get; set; }

        [Display(Name = "Apellido")]
        public string? Lastname { get; set; }

        [Display(Name = "Documento de identidad")]
        public string? Document { get; set; }

        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        //materias inscritas
        public List<SubjectViewModel> Subjects { get; set; } = new();
    }
}
