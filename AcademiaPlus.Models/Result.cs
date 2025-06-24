using System.Diagnostics.CodeAnalysis;

namespace AcademiaPlus.Models
{
    [ExcludeFromCodeCoverage]
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string ErrorMessage { get; private set; }

        public static Result Success() => new Result { IsSuccess = true };
        public static Result Failure(string error) => new Result { IsSuccess = false, ErrorMessage = error };
    }
}
