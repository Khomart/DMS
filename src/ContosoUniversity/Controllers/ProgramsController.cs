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
using ContosoUniversity.Models.Entities;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin, Professor")]
    public class ProgramsController : Controller
    {
        private readonly SchoolContext _context;

        public ProgramsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Programs
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Programs.Include(d => d.Department ).Where(i => i.Archived == false);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Programs/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department");
            //ViewData["DepartmentID"] = new SelectList(_context.Departments.Where(a => a.Archived == false), "DepartmentID", "Name");
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramID,DepartmentID,Title,Short")] UniversityProgram program)
        {
            if (ModelState.IsValid)
            {
                _context.Add(program);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", program.DepartmentID);

            //ViewData["DepartmentID"] = new SelectList(_context.Departments.Where(a => a.Archived == false), "FacultyID", "Name", program.DepartmentID);
            return View(program);
        }

        // GET: Programs/Edit/5
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var program = await _context.Programs
                .Include(i => i.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ProgramID == id);
            if (program == null)
            {
                return NotFound();
            }
            //We are creating Dropdown list, make sure archived department shown;

            //var departmentsList = _context.Departments.Include(p => p.Programs).Where(a => a.Archived == false && a.DepartmentID != program.DepartmentID);
            //if (program.Department.Archived == true)
            //    departmentsList.Append(program.Department);
            //ViewData["DepartmentID"] = new SelectList(departmentsList, "DepartmentID", "Name", program.DepartmentID);
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", program.DepartmentID);
            return View(program);
        }

        // POST: Programs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> Save(int? id, [Bind("Title","Short","DepartmentID")] UniversityProgram program)
        {
            if (id == null)
            {
                return NotFound();
            }

            var programToUpdate = await _context.Programs.Include(i => i.Department).SingleOrDefaultAsync(m => m.ProgramID == id);

            if (programToUpdate == null)
            {
                UniversityProgram deletedProgram = new UniversityProgram();
                await TryUpdateModelAsync(deletedProgram);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewData["DepartmentID"] = new SelectList(_context.Departments, "Id", "FullName", deletedProgram.DepartmentID);
                return View(deletedProgram);
            }

            if (await TryUpdateModelAsync<UniversityProgram>(
                programToUpdate,
                "",
                s => s.Title, s => s.Short, s => s.DepartmentID))
            {

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {

                //ViewData["DepartmentID"] = new SelectList(_context.Departments.Where(a => a.Archived == false), "DepartmentID", "Name", programToUpdate.DepartmentID);
                ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", programToUpdate.DepartmentID);
                return View(programToUpdate);
            }
        }

        // GET: Programs/Archive/5
        public async Task<IActionResult> Archive(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var program = await _context.Programs
                .Include(d => d.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ProgramID == id);
            if (program == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(program);
        }

        // POST: Programs/Archive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int ProgramID)
        {
            try
            {
                var programToArchive = await _context.Programs.SingleAsync(m => m.ProgramID == ProgramID);
                if (programToArchive != null)
                {
                    programToArchive.Archived = true;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("Archive", new { concurrencyError = true, id = ProgramID });
            }
        }

    }
}
