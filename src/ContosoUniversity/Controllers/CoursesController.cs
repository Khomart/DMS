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
using System;

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
            ViewBag.DepartmentID = PopulateDropdown.Populate(_context, "department");
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
        public async Task<IActionResult> EditPost(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _context.Courses
                .Include(i => i.Department)
                .SingleOrDefaultAsync(c => c.CourseID == id);
            if (courseToUpdate == null)
            {
                Console.WriteLine("An error have occured editing instance. Make sure that this professor entity exists.");
                return RedirectToAction("Index");
            }

            _context.Entry(courseToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Course>(courseToUpdate,
                "",
                c => c.Credits, c => c.DepartmentID, c => c.Title, c => c.ShortTitle, c => c.Active))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    // Using a NoTracking query means we get the entity but it is not tracked by the context
                    // and will not be merged with existing entities in the context.
                    var databaseEntity = await _context.Courses
                        .Include(i => i.Department)
                        .AsNoTracking()
                        .SingleAsync(d => d.CourseID == ((Course)exceptionEntry.Entity).CourseID);
                    var databaseEntry = _context.Entry(databaseEntity);

                    var databaseTitle = (string)databaseEntry.Property("Title").CurrentValue;
                    var proposedTitle = (string)exceptionEntry.Property("Title").CurrentValue;
                    if (databaseTitle != proposedTitle)
                    {
                        ModelState.AddModelError("Title", $"Current value: {databaseTitle}");
                    }
                    var databaseShortTitle = (string)databaseEntry.Property("ShortTitle").CurrentValue;
                    var proposedShortTitle = (string)exceptionEntry.Property("ShortTitle").CurrentValue;
                    if (databaseShortTitle != proposedShortTitle)
                    {
                        ModelState.AddModelError("ShortTitle", $"Current value: {databaseShortTitle}");
                    }
                    var databaseDepartmentID = (Int32)databaseEntry.Property("DepartmentID").CurrentValue;
                    var proposedDepartmentID = (Int32)exceptionEntry.Property("DepartmentID").CurrentValue;
                    if (databaseDepartmentID != proposedDepartmentID)
                    {
                        ModelState.AddModelError("DepartmentID", $"Current value: {databaseEntry.Entity.Department.Name}");
                    }
                    var databaseCredits = databaseEntry.Property("Credits").CurrentValue;
                    var proposedCredits = exceptionEntry.Property("Credits").CurrentValue;
                    if (databaseCredits != proposedCredits)
                    {
                        ModelState.AddModelError("Credits", $"Current value: {databaseCredits}");
                    }
                    var databaseActive = databaseEntry.Property("Active").CurrentValue;
                    var proposedActive = exceptionEntry.Property("Active").CurrentValue;
                    if (databaseActive != proposedActive)
                    {
                        ModelState.AddModelError("Active", $"Current value: {databaseEntry.Entity.Status}");
                    }

                    ModelState.AddModelError(string.Empty, "The Course entity you attempted to edit "
                            + "was modified by you or another user after you got the original values on this page. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Return button.");
                    courseToUpdate.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }
            }
            ViewBag.DepartmentID = PopulateDropdown.Populate(_context, "department", courseToUpdate.DepartmentID);
            return View(courseToUpdate);
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
            return _context.Courses.AsNoTracking().Any(e => e.CourseID == id);
        }

    }
}
