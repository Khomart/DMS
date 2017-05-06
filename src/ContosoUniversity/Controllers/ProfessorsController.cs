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
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> Index(int? id)
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
            //ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
            var professor = new RegProfModel();
            professor.Courses = new List<CourseAssignment>();
            PopulateAssignedCourseData(professor);
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", null, "Faculty.Name");
            ViewData["Semesters"] = PopulateDropdown.Populate(_context, "semester");
            ViewData["Courses"] = PopulateDropdown.Populate(_context, "course");
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
            string[] selectedCourses,
            string[] selectedSemesters)
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
                    DepartmentID = professor.DepartmentID,
                    Archived = false,
                };

                var result = await _userManager.CreateAsync(user, professor.Password);
                if (!result.Succeeded)
                {
                    var exceptionText = result.Errors.Aggregate("User Creation Failed - Identity Exception. Errors were: \n\r\n\r", (current, error) => current + (" - " + error + "\n\r"));
                    throw new Exception(exceptionText);
                }
                else await _userManager.AddToRoleAsync(user, "PROFESSOR");
                
                //if (selectedCourses != null)
                //{
                //    Professor tempuser = _context.Professors.FirstOrDefault(x => x.Email == user.Email);
                //    tempuser.Courses = new List<CourseAssignment>();
                //    //var prof = await _context.Professors.SingleOrDefaultAsync(m => m.Id == i.Id);
                //    var selectedCoursesHS = new List<string>(selectedCourses);
                //    var selectedSemesterHS = new List<string>(selectedSemesters);
                //    //var currentSem = await _context.Semesters.SingleOrDefaultAsync(i => i.Current == true);
                //    for(int i=0; i< Math.Max(selectedCoursesHS.Count(), selectedSemesterHS.Count()); i++)
                //    {
                //        int course, semester;
                //        if (Int32.TryParse(selectedCoursesHS[i], out course) && Int32.TryParse(selectedSemesterHS[i], out semester))
                //        {
                //            var assignment = new CourseAssignment { ProfessorID = tempuser.Id, CourseID = course, SemesterID = semester };
                //            tempuser.Courses.Add(assignment); 
                //        }
                //    }
                //    try
                //    {
                //        await _context.SaveChangesAsync();
                //    }
                //    catch (DbUpdateException /* ex */)
                //    {
                //        //Log the error (uncomment ex variable name and write a log.)
                //        ModelState.AddModelError("", "Unable to save changes. " +
                //            "Try again, and if the problem persists, " +
                //            "see your system administrator.");
                //    }
                //}
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y => y.Count > 0)
                                        .ToList();
            }
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", professor.DepartmentID, "Faculty.Name");
            ViewData["Semesters"] = PopulateDropdown.Populate(_context, "semester");
            ViewData["Courses"] = PopulateDropdown.Populate(_context, "course");
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
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses).ThenInclude(i => i.Semester)
                .Include(e => e.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id && m.Archived == false);
            if (professor == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(professor);
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", professor.DepartmentID, "Faculty.Name");
            ViewData["Semesters"] = PopulateDropdown.Populate(_context, "semester");
            ViewData["Courses"] = PopulateDropdown.Populate(_context, "course");
            //PopulateDropDownList("semester");
            //PopulateDropDownList("course");
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

        // POST: Professors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, int DepartmentID, string[] selectedCourses, string[] selectedSemesters)
        {
            if (id == null)
            {
                return NotFound();
            }

            Professor professorToUpdate = await _context.Professors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                .Include(i => i.Department)
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
                //UpdateProfessorCourses(selectedCourses, professorToUpdate);

                if (selectedCourses != null)
                {
                    var selectedCoursesHS = new List<string>(selectedCourses);
                    var selectedSemesterHS = new List<string>(selectedSemesters);
                    for (int i = 0; i < Math.Max(selectedCoursesHS.Count(), selectedSemesterHS.Count()); i++)
                    {

                        int course, semester;
                        if (Int32.TryParse(selectedCoursesHS[i], out course) && Int32.TryParse(selectedSemesterHS[i], out semester))
                        {
                            if (professorToUpdate.Courses.SingleOrDefault(d => d.SemesterID == semester && d.CourseID == course) == null)
                            {

                                var assignment = new CourseAssignment { ProfessorID = professorToUpdate.Id, CourseID = course, SemesterID = semester };
                                professorToUpdate.Courses.Add(assignment);
                            }
                        }
                    }
                    foreach (CourseAssignment cours in professorToUpdate.Courses)
                    {
                        bool net = true;
                        for (int i = 0; i < Math.Max(selectedCoursesHS.Count(), selectedSemesterHS.Count()); i++)
                            if (selectedCoursesHS[i].Contains(cours.CourseID.ToString()) && selectedSemesterHS[i].Contains(cours.SemesterID.ToString()))
                            {
                                net = false;;
                            }
                        if (net == true)
                        {
                            CourseAssignment courseToRemove = professorToUpdate.Courses.SingleOrDefault(i => i.CourseID == cours.CourseID && i.SemesterID == cours.SemesterID);
                            _context.Remove(courseToRemove);
                        }
                    }
                    professorToUpdate.DepartmentID = DepartmentID;
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
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", professorToUpdate.DepartmentID, "Faculty.Name");
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

        // GET: Instructors/Archive/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors.SingleOrDefaultAsync(m => m.Id == id && m.Archived == false);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // POST: Instructors/Archive/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Professor professor = await _context.Professors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .SingleAsync(i => i.Id == id);


            var departments = await _context.Departments
                .Where(d => d.ProfessorID == id)
                .ToListAsync();
            departments.ForEach(d => d.ProfessorID = null);
            var committies = await _context.Committees
                .Where(c => c.Chair.Id == professor.Id)
                .ToListAsync();
            committies.ForEach(c => c.Chair = null);
            var memberships = await _context.CommitieMembership
                .Where(m => m.ProfessorID == professor.Id)
                .ToListAsync();
            memberships.ForEach(m => { m.FinishedWork = true; m.EndDate = DateTime.Now; });
            professor.Archived = true;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ProfessorExists(int id)
        {
            return _context.Professors.Any(e => e.Id == id);
        }

        private void PopulateDropDownList(string type, object selected = null)
        {
            if (type.Equals("semester"))
            {
                var studentsQuery = from d in _context.Semesters
                                    where d.Archived == false
                                    orderby d.StartYear
                                    select d;
                var selectedArchived = _context.Semesters.SingleOrDefault(i => i.ID == (int)selected);
                if (selectedArchived != null && selectedArchived.Archived == true)
                    studentsQuery.Append(selectedArchived);
                ViewData["Semesters"] = new SelectList(studentsQuery.AsNoTracking(), "ID", "Title", selected);
            }
            else if (type.Equals("course"))
            {
                var coursesQuery = from d in _context.Courses
                                   where d.Archived == false
                                   orderby d.Title
                                   select d;
                var selectedArchived = _context.Courses.SingleOrDefault(i => i.CourseID == (int)selected);
                if (selectedArchived != null && selectedArchived.Archived == true)
                    coursesQuery.Append(selectedArchived);
                ViewData["Courses"] = new SelectList(coursesQuery, "CourseID", "ShortTitle");
            }
            else if (type.Equals("department"))
            {
                var departmentQuery = from d in _context.Departments.Include(i => i.Faculty)
                                   where d.Archived == false
                                   orderby d.Name
                                   select d;
                var selectedArchived = _context.Departments.SingleOrDefault(i => i.DepartmentID == (int)selected);
                if (selectedArchived != null && selectedArchived.Archived == true)
                    departmentQuery.Append(selectedArchived);
                ViewData["DepartmentID"] = new SelectList(departmentQuery, "DepartmentID", "Name", null, "Faculty.Name");
            }

        }
    }
}
