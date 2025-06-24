using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string Matter { get; set; }
        public string Code { get; set; }
        public int Credits { get; set; } = 0;

    }
}
