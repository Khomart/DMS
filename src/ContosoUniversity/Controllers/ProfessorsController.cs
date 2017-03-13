using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContosoUniversity.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin, Professor")]
    public class ProfessorsController : Controller
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;


        private readonly SchoolContext _context;

        public ProfessorsController(SchoolContext context,
                                    UserManager<IdentityUser<int>> userManager,
                                    SignInManager<IdentityUser<int>> signInManager,
                                    IEmailSender emailSender,
                                    ISmsSender smsSender,
                                    ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        // GET: Professors
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Index(int? id/*, int? courseID*/)
        {
            var viewModel = new ProfessorIndexData();
            viewModel.Professors = await _context.Professors
                  .Include(i => i.OfficeAssignment)
                  .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                  .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                  .OrderBy(i => i.LastName)
                  .ToListAsync();

            if (id != null)
            {
                ViewData["ProfessorID"] = id.Value;
                Professor professor = viewModel.Professors.Where(
                    i => i.Id == id.Value).Single();
                viewModel.Courses = professor.Courses.Select(s => s.Course);
            }

            //if (courseID != null)
            //{
            //    ViewData["CourseID"] = courseID.Value;
            //    _context.Enrollments
            //        .Include(i => i.Student)
            //        .Where(c => c.CourseID == courseID.Value).Load();
            //    viewModel.Enrollments = viewModel.Courses.Where(
            //        x => x.CourseID == courseID).Single().Enrollments;
            //}

            return View(viewModel);
        }

        // GET: Instructors/Details/5
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors.SingleOrDefaultAsync(m => m.Id == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // GET: Instructors/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
            var professor = new RegProfModel();
            professor.Courses = new List<CourseAssignment>();
            PopulateAssignedCourseData(professor);
            return View();
        }

        // POST: Instructors/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            RegProfModel professor, 
            string Passwordfirst, 
            string PasswordConfirm, 
            string[] selectedCourses)
        {


            if (ModelState.IsValid)
            {
                var user = new Professor
                {
                    UserName = professor.Email,
                    Email = professor.Email,
                    FirstMidName = professor.FirstMidName,
                    LastName = professor.LastName,
                    OfficeAssignment = professor.OfficeAssignment,
                    Employment = new DepartmentEmploynment()
                    {
                        DepartmentID = professor.DepartmentID,
                    },
                };

                var result = await _userManager.CreateAsync(user, professor.Password);
                if (!result.Succeeded)
                {
                    var exceptionText = result.Errors.Aggregate("User Creation Failed - Identity Exception. Errors were: \n\r\n\r", (current, error) => current + (" - " + error + "\n\r"));
                    throw new Exception(exceptionText);
                }
                else await _userManager.AddToRoleAsync(user, "PROFESSOR");
                if (selectedCourses != null)
                {
                    Professor tempuser = _context.Professors.FirstOrDefault(x => x.Email == user.Email);
                    tempuser.Courses = new List<CourseAssignment>();
                    //var prof = await _context.Professors.SingleOrDefaultAsync(m => m.Id == i.Id);
                    var selectedCoursesHS = new HashSet<string>(selectedCourses);
                    var currentSem = await _context.Semesters.SingleOrDefaultAsync(i => i.Current == true);
                    foreach (var course in _context.Courses)
                    {
                        if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                        {
                            var huynya = new CourseAssignment { ProfessorID = tempuser.Id, CourseID = course.CourseID, SemesterID = currentSem.ID };
                            tempuser.Courses.Add(huynya);    
                        }
                    }
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y => y.Count > 0)
                                        .ToList();
            }
            return View(professor);
        }




        // GET: Instructors/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses).ThenInclude(i => i.Course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);
            if (professor == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(professor);
            return View(professor);
        }

        private void PopulateAssignedCourseData(Professor professor)
        {
            var allCourses = _context.Courses;
            var professorCourses = new HashSet<int>(professor.Courses.Select(c => c.Course.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = professorCourses.Contains(course.CourseID)
                });
            }
            ViewData["Courses"] = viewModel;
        }
        private void PopulateAssignedCourseData(RegProfModel professor)
        {
            var allCourses = _context.Courses.Where(i => i.Active == true);
            var professorCourses = new HashSet<int>(professor.Courses.Select(c => c.Course.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = professorCourses.Contains(course.CourseID)
                });
            }
            ViewData["Courses"] = viewModel;
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professorToUpdate = await _context.Professors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<Professor>(
                professorToUpdate,
                "",
                i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(professorToUpdate.OfficeAssignment?.Location))
                {
                    professorToUpdate.OfficeAssignment = null;
                }
                UpdateProfessorCourses(selectedCourses, professorToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction("Index");
            }
            return View(professorToUpdate);
        }

        private void UpdateProfessorCourses(string[] selectedCourses, Professor professorToUpdate)
        {
            if (selectedCourses == null)
            {
                professorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var professorCourses = new HashSet<int>
                (professorToUpdate.Courses.Select(c => c.Course.CourseID));
            var currentSem =  _context.Semesters.SingleOrDefault(i => i.Current == true);
            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!professorCourses.Contains(course.CourseID))
                    {
                        professorToUpdate.Courses.Add(new CourseAssignment { ProfessorID = professorToUpdate.Id, CourseID = course.CourseID, SemesterID = currentSem.ID });
                    }
                }
                else
                {

                    if (professorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove = professorToUpdate.Courses.SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }

        // GET: Instructors/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors.SingleOrDefaultAsync(m => m.Id == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // POST: Instructors/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Professor professor = await _context.Professors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .SingleAsync(i => i.Id == id);


            var departments = await _context.Departments
                .Where(d => d.ProfessorID == id)
                .ToListAsync();
            departments.ForEach(d => d.ProfessorID = null);

            _context.Professors.Remove(professor);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProfessorExists(int id)
        {
            return _context.Professors.Any(e => e.Id == id);
        }
    }
}
