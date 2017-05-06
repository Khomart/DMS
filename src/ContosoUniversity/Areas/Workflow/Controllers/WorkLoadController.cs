using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using ContosoUniversity.Areas.Workflow.Models.RequestModels;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace ContosoUniversity.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    [Authorize(Roles = "Admin, Professor")]
    public class WorkLoadController : Controller
    {
        private readonly SchoolContext _context;

        public WorkLoadController(SchoolContext context, IHostingEnvironment environment, UserManager<IdentityUser<int>> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;

        }
        private IHostingEnvironment _environment;
        public UserManager<IdentityUser<int>> _userManager { get; private set; }

        // GET: Committees
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Index(int? status)
        {
            if (status == null) status = 1;
            IEnumerable<Workload> workloads = new List<Workload>();
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User.IsInRole("Admin"))
            {
                switch (status)
                {
                    case 1: workloads = await _context.Workloads.Include(p => p.Professor).Where(i => i.Reviewed == false).ToListAsync(); break;
                    case 2: workloads = await _context.Workloads.Include(p => p.Professor).Where(i => i.Reviewed == true).ToListAsync(); break;
                    case 3: workloads = await _context.Workloads.Include(p => p.Professor).ToListAsync(); break;
                }
            }
            else
            {
                workloads = await _context.Workloads.Include(p => p.Professor).Where(i => i.ProfessorID == user.Id && i.Finished).ToListAsync();
                if (_context.Workloads.Where(i => i.ProfessorID == user.Id && i.Year == DateTime.Today.Year && i.Finished).Any())
                    ViewData["Open"] = false;
                else
                    ViewData["Open"] = true;
            }
            ViewData["ID"] = user.Id;
            return View(workloads);
        }

        [Authorize(Roles = "Professor")]
        [ActionName("WorkloadForm")]
        public async Task<IActionResult> WorkloadForm(int? id)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            Workload formStarted = new Workload();
            if (!_context.Workloads.Where(i => i.ProfessorID == user.Id && i.Year == DateTime.Today.Year && i.Finished == false).Any())
            {
                ViewData["Year"] = DateTime.Today.Year;
                return View();
            }
            else
            {
                formStarted = _context.Workloads.Where(i => i.ProfessorID == user.Id && i.Year == DateTime.Today.Year && i.Finished == false).SingleOrDefault();
                ViewData["Year"] = DateTime.Today.Year;
            }
            return View(formStarted);
        }

        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("saveForm")]
        public async Task<IActionResult> SaveFormAsync(Workload model)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (_context.Workloads.Any(i=> i.Year == DateTime.Today.Year && i.ProfessorID == user.Id && i.Finished)) return Json("error");
            var worklToUpdate = await _context.Workloads.SingleOrDefaultAsync(i => i.Year == model.Year && i.ProfessorID == model.ProfessorID && i.Finished != true && i.Reviewed != true);
            if (worklToUpdate != null)
            {
                _context.Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                model.ProfessorID = user.Id;
                model.Year = DateTime.Today.Year;
                _context.Workloads.Add(model);
               await _context.SaveChangesAsync();
            }
            return Json("Success");
        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("WorkloadForm")]
        public async Task<IActionResult> WorkloadFormSubmit(Workload model)
        {
            
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            model.Year = DateTime.Today.Year;
            model.ProfessorID = user.Id;
            //model.SemesterID = _context.Semesters.Single(i => i.Current).ID;
            model.Finished = true;
            model.Reviewed = false;
            if (ModelState.IsValid)
            {
                _context.Workloads.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [ActionName("Evaluate")]
        public async Task<IActionResult> WorkloadEvaluation(int? id)
        {
            if (id == null)
            {
                return BadRequest();      
            }
            var workload = await _context.Workloads.SingleOrDefaultAsync(i => i.WorkloadID == id);
            if (workload == null)
            {
                return NotFound();
            }
            if (workload.Reviewed)
            {
                RedirectToAction("Index");
            }
            return View(workload);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost,ActionName("Evaluate")]
        public async Task<IActionResult> Evaluate(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var evaluatedWorkload = _context.Workloads.Single(i => i.WorkloadID == id);
            if (evaluatedWorkload.Reviewed)
            {
                RedirectToAction("Index");
            }
            if (await TryUpdateModelAsync(
                evaluatedWorkload,
                "",
                i => i.Duties, i => i.Notes))
            {
                evaluatedWorkload.Reviewed = true;
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException /*dex*/)
                {
                    Response.StatusCode = 500;
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(evaluatedWorkload);
        }

        [Authorize(Roles = "Admin, Professor")]
        [ActionName("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var evaluatedWorkload = await _context.Workloads.AsNoTracking().SingleAsync(i => i.WorkloadID == id);
            if (!evaluatedWorkload.Reviewed)
            {
                RedirectToAction("Index");
            }
            return View(evaluatedWorkload);
        }

    }
}

