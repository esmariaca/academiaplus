using AcademiaPlus.Models;
using AcademiaPlus.ViewModels;
using AcademiaPlus.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaPlus.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ISubjectService _subjectService;

        public StudentController(IStudentService studentService, ISubjectService subjectService)
        {
            _studentService = studentService;
            _subjectService = subjectService;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllAsync();
            return View(students.Select(s => new StudentViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Lastname = s.Lastname,
                Document = s.Document,
                Email = s.Email,
                Subjects = s.Registrations.Select(r => new SubjectViewModel
                {
                    Id = r.Subject.Id,
                    Matter = r.Subject.Matter,
                    Code = r.Subject.Code
                }).ToList()
            }).ToList());
        }
        public async Task<IActionResult> Create()
        {
            var subjects = await _subjectService.GetAllAsync();

            ViewBag.Subjects = subjects?.Select(s => new SubjectViewModel
            {
                Matter = s.Matter,
                Code = s.Code
            }).ToList() ?? new List<SubjectViewModel>();

            return View(new StudentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState tiene errores:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"➡ {error.ErrorMessage}");
                }

                ViewBag.Subjects = await _subjectService.GetAllAsync();
                return View(model);
            }

            var student = new Student
            {
                Name = model.Name,
                Lastname = model.Lastname,
                Document = model.Document,
                Email = model.Email
            };

            var result = await _studentService.CreateAsync(student);
            Console.WriteLine($"Nombre recibido: {model.Name}");
            Console.WriteLine($"Apellido recibido: {model.Lastname}");
            Console.WriteLine($"Documento recibido: {model.Document}");
            Console.WriteLine($"Email recibido: {model.Email}");

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RegisterSubject(int id)
        {
            var students = await _studentService.GetAllAsync();
            var student = students.FirstOrDefault(t => t.Id == id);
            var subjects = await _subjectService.GetAllAsync();

            var model = new RegistrationViewModel
            {
                StudentId = student.Id,
                Students = new List<StudentViewModel>
        {
            new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Lastname = student.Lastname,
                Document = student.Document,
                Email = student.Email
            }
        },
                Subjects = subjects.Select(sub => new SubjectViewModel
                {
                    Id = sub.Id,
                    Matter = sub.Matter,
                    Code = sub.Code,
                    Credits = sub.Credits
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSubject(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "❌ Error en el formulario. Verifica los datos.";
                await LoadStudentsAndSubjects(model);
                return View(model);
            }

            var result = await _studentService.RegisterSubjectAsync(model.StudentId, model.SubjectId);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                TempData["Message"] = $"❌ {result.ErrorMessage}";
                await LoadStudentsAndSubjects(model);
                return View(model);
            }

            // Verificar que la inscripción realmente fue guardada en la base de datos
            var student = await _studentService.GetByIdAsync(model.StudentId);
            bool subjectRegistered = student.Registrations.Any(r => r.SubjectId == model.SubjectId);

            if (!subjectRegistered)
            {
                TempData["Message"] = "❌ Hubo un problema al inscribir la materia. Intenta nuevamente.";
                await LoadStudentsAndSubjects(model);
                return View(model);
            }

            TempData["Message"] = "✅ Materia inscrita correctamente.";
            return RedirectToAction("Index");
        }

        // Método para cargar listas de estudiantes y materias
        private async Task LoadStudentsAndSubjects(RegistrationViewModel model)
        {
            model.Students = (await _studentService.GetAllAsync())
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Lastname = s.Lastname,
                    Document = s.Document,
                    Email = s.Email
                }).ToList();

            model.Subjects = (await _subjectService.GetAllAsync())
                .Select(s => new SubjectViewModel
                {
                    Id = s.Id,
                    Matter = s.Matter,
                    Code = s.Code,
                    Credits = s.Credits
                }).ToList();
        }
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var model = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Lastname = student.Lastname,
                Document = student.Document,
                Email = student.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingStudent = await _studentService.GetByIdAsync(model.Id);
            if (existingStudent == null)
            {
                return NotFound();
            }
            existingStudent.Name = string.IsNullOrWhiteSpace(model.Name) ? existingStudent.Name : model.Name;
            existingStudent.Lastname = string.IsNullOrWhiteSpace(model.Lastname) ? existingStudent.Lastname : model.Lastname;
            existingStudent.Document = string.IsNullOrWhiteSpace(model.Document) ? existingStudent.Document : model.Document;
            existingStudent.Email = string.IsNullOrWhiteSpace(model.Email) ? existingStudent.Email : model.Email;


            var result = await _studentService.UpdateAsync(existingStudent);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var model = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Lastname = student.Lastname,
                Document = student.Document,
                Email = student.Email
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _studentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnregisterSubject(int studentId, int subjectId)
        {
            var result = await _studentService.UnsubscribeAsync(studentId, subjectId);
            if (!result.IsSuccess)
            {
                TempData["Message"] = $"❌ {result.ErrorMessage}";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "✅ Materia eliminada correctamente.";
            return RedirectToAction("Index");
        }
    }
}
