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

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin, Professor")]
    public class CommitteesController : Controller
    {
        private readonly SchoolContext _context;

        public CommitteesController(SchoolContext context)
        {
            _context = context;    
        }

        // GET: Committees
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Committees.Include(c => c.Chair).Include(c => c.Department);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Committees/Details/5
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committee = await _context.Committees.SingleOrDefaultAsync(m => m.CommitteeID == id);
            if (committee == null)
            {
                return NotFound();
            }

            return View(committee);
        }

        // GET: Committees/Create
        public IActionResult Create()
        {
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "FullName");
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "Name");
            return View();
        }

        // POST: Committees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommitteeID,DepartmentID,ProfessorID,StartDate,Title")] Committee committee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(committee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "Discriminator", committee.ProfessorID);
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", committee.DepartmentID);
            return View(committee);
        }

        // GET: Committees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committee = await _context.Committees.SingleOrDefaultAsync(m => m.CommitteeID == id);
            if (committee == null)
            {
                return NotFound();
            }
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "Discriminator", committee.ProfessorID);
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", committee.DepartmentID);
            return View(committee);
        }

        // POST: Committees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommitteeID,DepartmentID,ProfessorID,StartDate,Title")] Committee committee)
        {
            if (id != committee.CommitteeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(committee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommitteeExists(committee.CommitteeID))
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
            ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "Discriminator", committee.ProfessorID);
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", committee.DepartmentID);
            return View(committee);
        }

        // GET: Committees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committee = await _context.Committees.SingleOrDefaultAsync(m => m.CommitteeID == id);
            if (committee == null)
            {
                return NotFound();
            }

            return View(committee);
        }

        // POST: Committees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var committee = await _context.Committees.SingleOrDefaultAsync(m => m.CommitteeID == id);
            _context.Committees.Remove(committee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CommitteeExists(int id)
        {
            return _context.Committees.Any(e => e.CommitteeID == id);
        }
    }
}
