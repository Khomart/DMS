using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.UniversityFunctionalityModels.Models;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SemestersController : Controller
    {
        private readonly SchoolContext _context;

        public SemestersController(SchoolContext context)
        {
            _context = context;    
        }

        // GET: Semesters
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Semesters.ToListAsync());
        }

        // GET: Semesters/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters.SingleOrDefaultAsync(m => m.ID == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // GET: Semesters/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<SelectListItem> semesters = new List<SelectListItem>()
            {

                new SelectListItem { Text = "Summer First", Value = "1" },
                new SelectListItem { Text = "Summer Second", Value = "2" },
                new SelectListItem { Text = "Autumn", Value = "3" },
                new SelectListItem { Text = "Winter", Value = "4" },
            };
            ViewBag.Semesters = new SelectList(semesters, "Value", "Text");
            return View();
        }

        // POST: Semesters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Current,Open,Season,StartYear,StartingDate")] Semester semester)
        {
            if (ModelState.IsValid)
            {
                var sems = _context.Semesters;
                foreach (var sem in sems)
                    if (sem.StartYear == semester.StartYear && sem.Season == semester.Season)
                    {
                        List<SelectListItem> semesters = new List<SelectListItem>()
                        {

                            new SelectListItem { Text = "Summer First", Value = "1" },
                            new SelectListItem { Text = "Summer Second", Value = "2" },
                            new SelectListItem { Text = "Autumn", Value = "3" },
                            new SelectListItem { Text = "Winter", Value = "4" },
                        };
                        ViewBag.Semesters = new SelectList(semesters, "Value", "Text");
                        return View(semester);
                    }
                var oldCurrent = await sems.SingleOrDefaultAsync(i => i.Current == true);
                if (semester.Current == true && oldCurrent != null)
                {
                    oldCurrent.Current = false;
                }
                _context.Add(semester);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(semester);
        }

        // GET: Semesters/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters.SingleOrDefaultAsync(m => m.ID == id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        // POST: Semesters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPost(int? ID)
        {
            if (ID == null)
            {
                return NotFound();
            }
            var semester = await _context.Semesters.SingleOrDefaultAsync(i => i.ID == ID);

            if (ModelState.IsValid)
            {
                var oldCurrent = await _context.Semesters.SingleOrDefaultAsync(i => i.Current == true);
                if((oldCurrent == null || oldCurrent.ID == int.Parse(Request.Form["ID"])) && bool.Parse(Request.Form["current"][0]) == false)
                {
                    return RedirectToAction("Index");
                }
                if (bool.Parse(Request.Form["current"][0]) == true && oldCurrent != null)
                {
                    oldCurrent.Current = false;
                }
                if (await TryUpdateModelAsync<Semester>(semester,
                "",
                c => c.Current, c => c.Open))
                    try
                    {
                        _context.Update(semester);
                        if (oldCurrent == null && semester.Current == false) semester.Current = true;
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SemesterExists(semester.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    } 
                return RedirectToAction("Index");
            }
            return View(semester);
        }

        // GET: Semesters/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters.SingleOrDefaultAsync(m => m.ID == id);
            if (semester == null)
            {
                return NotFound();
            }
            if (semester.Current == true || semester.StartYear <= DateTime.Today.Year)
            {
                return RedirectToAction("Index");
            }
            return View(semester);
        }

        // POST: Semesters/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var semester = await _context.Semesters.SingleOrDefaultAsync(m => m.ID == id);
            if (semester.Current == true || semester.StartYear <= DateTime.Today.Year)
            {
                return RedirectToAction("Index");
            }
            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SemesterExists(int id)
        {
            return _context.Semesters.Any(e => e.ID == id);
        }
    }
}
