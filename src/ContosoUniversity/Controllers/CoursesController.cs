using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ContosoUniversity.Models.Entities;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin, Professor, Student, Registered")]
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context,
            UserManager<IdentityUser<int>> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public UserManager<IdentityUser<int>> _userManager { get; private set; }
        public RoleManager<IdentityRole<int>> _roleManager { get; private set; }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var courses = _context.Courses
                    .Include(c => c.Department)
                    .AsNoTracking();
                return View(await courses.ToListAsync());

            }
            else
            {
                var courses = _context.Courses
                    .Include(c => c.Department)
                    .Where(i => i.Active == true)
                    .AsNoTracking();
                return View(await courses.ToListAsync());

            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Credits,DepartmentID,Title,ShortTitle")] Course course)
        {
            course.Active = true;
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = PopulateDropdown.Populate(_context, "department", course.DepartmentID);
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(i => i.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            ViewBag.DepartmentID = PopulateDropdown.Populate(_context, "department");
            //PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _context.Courses
                .Include(i => i.Department)
                .SingleOrDefaultAsync(c => c.CourseID == id);

            if (await TryUpdateModelAsync<Course>(courseToUpdate,
                "",
                c => c.Credits, c => c.DepartmentID, c => c.Title, c => c.ShortTitle, c => c.Active))
            {
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
            ViewBag.DepartmentID = PopulateDropdown.Populate(_context, "department", courseToUpdate.DepartmentID);
            //PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   where d.Archived == false
                                   orderby d.Name
                                   select d;
            var selectedArchived = _context.Departments.SingleOrDefault(i => i.DepartmentID == (int)selectedDepartment);
            if (selectedArchived != null && selectedArchived.Archived == true)
                departmentsQuery.Append(selectedArchived);
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UpdateCourseCredits()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewData["RowsAffected"] =
                    await _context.Database.ExecuteSqlCommandAsync(
                        "UPDATE Course SET Credits = Credits * {0}",
                        parameters: multiplier);
            }
            return View();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }

    }
}
