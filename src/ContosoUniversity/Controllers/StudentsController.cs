using System;
using System.Collections.Generic;
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
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin, Professor, Registered, Student")]
    public class StudentsController : Controller
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context, 
            UserManager<IdentityUser<int>> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Students
        [Authorize(Roles = "Admin, Professor, Registered, Student")]
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageSize,
            int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ProgramSortParm"] = sortOrder == "Program" ? "program_desc" : "Program";
            ViewData["VerifiedSortParm"] = sortOrder == "Verification" ? "verification_desc" : "Verification";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var students = from s in _context.Students.Include(p => p.Program).Where(s => s.Archived == false)
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "program_desc":
                    students = students.OrderByDescending(s => s.Program);
                    break;
                case "Program":
                    students = students.OrderBy(s => s.Program);
                    break;
                case "Verification":
                    students = students.OrderBy(s => Convert.ToString(s.Approved));
                    break;
                case "verification_desc":
                    students = students.OrderByDescending(s => Convert.ToString(s.Approved));
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            if (pageSize == null) pageSize = 25;
            var dictionary = new Dictionary<int, string>
            {
                { 10, "10" },
                { 25, "25" },
                { 50, "50" }
            };
            ViewData["PSize"] = pageSize;
            ViewBag.PageSize = new SelectList(dictionary, "Key", "Value",pageSize);
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), page ?? 1, (int)pageSize));
        }
        // GET: Students/Details/5
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id && m.Archived == false);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ProgramID"] = PopulateDropdown.Populate(_context, "program");
            //ViewData["ProgramID"] = new SelectList(_context.Programs.Where(a => a.Archived == false), "ProgramID", "Title");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
           /* [Bind("Program,FirstMidName,LastName")] */RegStudModel student)
        {
            if (ModelState.IsValid)
            {
                var user = new Student
                {
                    UserName = student.Email,
                    Email = student.Email,
                    FirstMidName = student.FirstMidName,
                    LastName = student.LastName,
                    ProgramID = student.ProgramID,
                    Archived = false,
                };

                var result = await _userManager.CreateAsync(user, student.Password);
                if (!result.Succeeded)
                {
                    var exceptionText = result.Errors.Aggregate("User Creation Failed - Identity Exception. Errors were: \n\r\n\r", (current, error) => current + (" - " + error + "\n\r"));
                    throw new Exception(exceptionText);
                }
                else await _userManager.AddToRoleAsync(user, "STUDENT");
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                        .Where(y => y.Count > 0)
                                        .ToList();
            }
            ViewData["ProgramID"] = PopulateDropdown.Populate(_context, "program", student.ProgramID);
            //ViewData["ProgramID"] = new SelectList(_context.Programs.Where(a => a.Archived == false), "ProgramID", "Title",student.ProgramID);
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.Include(p => p.Program).SingleOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }
            var programsList = _context.Programs.Where(a => a.Archived == false && a.ProgramID != student.ProgramID);
            if (student.Program.Archived == true)
                programsList.Append(student.Program);
            ViewData["ProgramID"] = PopulateDropdown.Populate(_context, "program", student.ProgramID);
            //ViewData["ProgramID"] = new SelectList(programsList, "ProgramID", "Title",student.ProgramID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.SingleOrDefaultAsync(s => s.Id == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.ProgramID, s => s.Email, s => s.Approved, s => s.EmailConfirmed))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            ViewData["ProgramID"] = PopulateDropdown.Populate(_context, "program", studentToUpdate.ProgramID);
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Archive(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            var student = await _context.Students
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
        }
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
