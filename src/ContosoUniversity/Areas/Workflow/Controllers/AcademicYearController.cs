using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Models.Entities;
using ContosoUniversity.Areas.Workflow.Models.AcademicYear;

namespace ContosoUniversity.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    [Authorize(Roles = "Admin")]
    public class AcademicYearController : Controller
    {
        private readonly SchoolContext _context;

        public AcademicYearController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Semesters
        [Authorize(Roles = "Admin")]
        public  IActionResult Index()
        {
            return RedirectToAction("StartYear");
            //List<AcademicYear> Years = new List<AcademicYear>();
            //foreach(var semester in _context.Semesters.AsNoTracking())
            //    if (Years == null)
            //    {
            //        AcademicYear year = new AcademicYear
            //        {
            //            StartingDate = semester.StartYear,
            //            EndDate = semester.EndYear,
            //            Semesters = new List<Semester>
            //            {
            //                 semester
            //            }
            //        };
            //        Years.Add(year);
            //    }
            //    else
            //    {
            //        bool searchFlag = false;
            //        foreach (var year in Years)
            //        {
            //            if (year.StartingDate == semester.StartYear)
            //            {
            //                year.Semesters.Add(semester);
            //                searchFlag = true;
            //                break;
            //            }
            //        }
            //        if (searchFlag == false)
            //        {
            //            AcademicYear year = new AcademicYear
            //            {
            //                StartingDate = semester.StartYear,
            //                EndDate = semester.EndYear,
            //                Semesters = new List<Semester>
            //                {
            //                     semester
            //                }
            //            };
            //            Years.Add(year);
            //        }
            //    }
            //Years.OrderBy(i => i.StartingDate);
            //    return View(Years);
        }

        [Authorize(Roles = "Admin")]
        [ActionName("StartYear")]
        public async Task<IActionResult> NewYearCreationFirst(int? id)
        {
            var year = 0;
            if (await _context.Semesters.AnyAsync())
                foreach (var semester in _context.Semesters)
                    if (semester.StartYear > year) year = semester.StartYear;
                    else;
            else year = DateTime.Today.Year - 1;
            ViewData["year"] = (year+1) + " - " + (year + 2);
            ViewData["hiddenActualYear"] = year + 1;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost,ActionName("createYear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewYearCreationSubmit(Year Model)
        {
            if (ModelState.IsValid)
            {
                Semester summer1 = new Semester()
                {
                    Archived = false,
                    Current = false,
                    Open = false,
                    Season = Term.Summer_I,
                    StartingDate = Model.Summer1,
                    StartYear = Model.YearValue,
                    EndingDate = Model.Summer1End
                };
                _context.Semesters.Add(summer1);
                Semester summer2 = new Semester()
                {
                    Archived = false,
                    Current = false,
                    Open = false,
                    Season = Term.Summer_II,
                    StartingDate = Model.Summer2,
                    StartYear = Model.YearValue,
                    EndingDate = Model.Summer2End
                };
                _context.Semesters.Add(summer2);
                Semester summer3 = new Semester()
                {
                    Archived = false,
                    Current = false,
                    Open = false,
                    Season = Term.Summer_III,
                    StartingDate = Model.Summer_long,
                    StartYear = Model.YearValue,
                    EndingDate = Model.Summer_longEnd
                };
                _context.Semesters.Add(summer3);
                Semester fall = new Semester()
                {
                    Archived = false,
                    Current = false,
                    Open = false,
                    Season = Term.Autumn,
                    StartingDate = Model.Fall,
                    StartYear = Model.YearValue,
                    EndingDate = Model.FallEnd
                };
                _context.Semesters.Add(fall);
                Semester winter = new Semester()
                {
                    Archived = false,
                    Current = false,
                    Open = false,
                    Season = Term.Winter,
                    StartingDate = Model.Winter,
                    StartYear = Model.YearValue,
                    EndingDate = Model.WinterEnd
                };
                _context.Semesters.Add(winter);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return View(Model);
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
                if ((oldCurrent == null || oldCurrent.ID == int.Parse(Request.Form["ID"])) && bool.Parse(Request.Form["current"][0]) == false)
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

