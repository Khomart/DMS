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
using ContosoUniversity.Models.SchoolViewModels;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ContosoUniversity.Models.Entities;
using ContosoUniversity.Areas.Workflow.Models.CommitteesViews;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static ContosoUniversity.Models.SchoolViewModels.ChoseDates;
using ContosoUniversity.Areas.Workflow.Models.RequestModels;

namespace ContosoUniversity.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    [Authorize(Roles = "Admin, Professor")]
    public class RequestController : Controller
    {
        private readonly SchoolContext _context;

        public RequestController(SchoolContext context, IHostingEnvironment environment, UserManager<IdentityUser<int>> userManager)
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
            string[] breadcr = Routes.ReturnRoutes(ControllerContext);
            if (status == null) status = 1;
            List<TeachingRequest> requests = new List<TeachingRequest>();
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User.IsInRole("Admin"))
            {
                switch (status)
                {
                    case 1: requests = await _context.TeachingRequests.Where(i => i.Approved == false).Include(s => s.SemesterForAssignment).Include(p => p.ProfessorEntity).ToListAsync(); break;
                    case 2: requests = await _context.TeachingRequests.Where(i => i.Approved == true).Include(s => s.SemesterForAssignment).Include(p => p.ProfessorEntity).ToListAsync(); break;
                    case 3: requests = await _context.TeachingRequests.Include(s => s.SemesterForAssignment).Include(p => p.ProfessorEntity).ToListAsync(); break;
                }
            }
            else
            {
                requests = await _context.TeachingRequests.Include(s => s.SemesterForAssignment).Include(p => p.ProfessorEntity).Where(i=> i.ProfessorID == user.Id).ToListAsync();
            }

            List<Semester> semesters = new List<Semester>();
            semesters = await _context.Semesters.ToListAsync();

            RequestView view = new RequestView()
            {
                Requests = requests,
                Semesters = semesters,
                UserID = user.Id,
            };
            ViewData["ID"] = user.Id;
            return View(view);
        }

        // GET: NewRequest
        [Authorize(Roles = "Professor")]
        [ActionName("RequestSemester")]
        public async Task<IActionResult> MakeRequest(int? id)
        {
            if (id == null) return NotFound();
            List<TeachingRequest> myRq = _context.TeachingRequests.Include(s=> s.SemesterForAssignment).Where(i => i.ProfessorID == (int)id).ToList();
            List<Semester> semesters = await _context.Semesters.Where(i => i.Open).ToListAsync();
            foreach (TeachingRequest sem in myRq)
            {
                if (semesters.Contains(sem.SemesterForAssignment))
                {
                    semesters.Remove(sem.SemesterForAssignment);
                }
            }
            ViewData["SemesterID"] = new SelectList(semesters, "ID", "Title");
            return View(semesters);
        }
        [Authorize(Roles = "Professor")]
        [ActionName("CourseLoad")]
        public async Task<IActionResult> MakeRequest2(int? semID)
        {
            if (semID == null || !_context.Semesters.Any(i => i.ID == semID && i.Open == true)) return NotFound();
            else
            {
                IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
                CoursesToChoose request = new CoursesToChoose()
                {
                    SemesterID = (int)semID,
                    ProfessorID = user.Id, 
                };
                var fullCourseList = await _context.Courses.Include(t => t.Department).Where(i => i.Active == true).AsNoTracking().ToListAsync();
                List<ChoosenCourse> somelist = new List<ChoosenCourse>();
                foreach (var s in fullCourseList)
                {
                    ChoosenCourse course = new ChoosenCourse()
                    {
                        SelectedCourses = s,
                        //CourseID = s.CourseID,
                        Checked = false,
                        Choice = (Desire)1,
                    };
                    somelist.Add(course);
                }
                request.Courses = somelist;
                return View("RequestCourses", request);
            }
        }

        [Authorize(Roles = "Professor")]
        [HttpPost,ActionName("SubmitRequest")]
        public async Task<IActionResult> RequestCourses(CoursesToChoose model, FormCollection formCollection)
        {
            foreach (ChoosenCourse item in model.Courses)
            {
                item.SelectedCourses = await _context.Courses.Where(i => i.CourseID == item.SelectedCourses.CourseID).AsNoTracking().SingleOrDefaultAsync();
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                //var request = HttpContext.Session.GetObjectFromJson<TeachingRequest>("request");
                //if (_context.TeachingRequests.Any(i => i.ProfessorID == user.Id && i.SemesterID == request.SemesterID))
                //    return RedirectToAction("AccessDenied", "Account");
                TeachingRequest request = new TeachingRequest();
                request.SemesterID = model.SemesterID;
                request.ProfessorID = model.ProfessorID;
                request.Approved = false;
                request.ProfessorEntity = _context.Professors.Single(i => i.Id == model.ProfessorID);

                request.ListOfCourses = new List<CoursePreference>();
                foreach (ChoosenCourse item in model.Courses)
                    if (item.Checked == true)
                    {
                        //somelist.Add(item.SelectedCourses);
                        _context.Courses.Attach(item.SelectedCourses);
                        request.ListOfCourses.Add(new CoursePreference { Choice = item.Choice, CourseID = item.SelectedCourses.CourseID, Course = item.SelectedCourses });
                    }
                request.ProfessorEntity = await _context.Professors.Include(i => i.Department).Where(i => i.Id == request.ProfessorID).SingleOrDefaultAsync();
                _context.Professors.Attach(request.ProfessorEntity);
                //request.ListOfCourses = somelist;
                _context.Add(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        //admin actions
        [Authorize(Roles = "Admin")]
        [ActionName("RequestReview")]
        public async Task<IActionResult> RequestReview(int? semesterID, int? professorID)
        {
            if ((semesterID == null) || (professorID == null))
            {
                return NotFound();
            }

            var fullCourseList = await _context.Courses.Include(t => t.Department).Where(i => i.Active == true).AsNoTracking().ToListAsync();
            List<GivenCourse> somelist = new List<GivenCourse>();
            foreach (var s in fullCourseList)
            {
                GivenCourse course = new GivenCourse()
                {
                    SelectedCourses = s,
                    //CourseID = s.CourseID,
                    Checked = false,
                };
                somelist.Add(course);
            }

            RequestReview review = new RequestReview()
            {
                Request = await _context.TeachingRequests
                .Where(i => i.SemesterID == semesterID && i.ProfessorID == professorID)
                .Include(c => c.ListOfCourses)
                .ThenInclude(c => c.Course)
                .SingleOrDefaultAsync(),

                Courses = somelist,
            };

            ViewData["SemesterID"] = semesterID;
            ViewData["ProfessorID"] = professorID;
            return View(review);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("GiveLoad")]
        public async Task<IActionResult> GiveLoad(RequestReview model)
        {
            TeachingRequest request = await _context.TeachingRequests
                .Where(i => i.SemesterID == model.Request.SemesterID && i.ProfessorID == model.Request.ProfessorID)
                .SingleOrDefaultAsync();
            request.GivenCourses = new List<Course>();
            var givenCourses = model.Courses.Where(i => i.Checked == true).ToList();
            var courses = new List<Course>();
            foreach (GivenCourse s in givenCourses)
            {
                courses.Add(s.SelectedCourses);
            }
            _context.Courses.AttachRange(courses);

            request.GivenCourses = courses;
            request.Approved = true;

            List<CourseAssignment> assigments = new List<CourseAssignment>();
            foreach (var course in courses)
            {
                assigments.Add(new CourseAssignment()
                {
                    AssignmentDate = DateTime.Today,
                    CourseID = course.CourseID,
                    ProfessorID = request.ProfessorID,
                    SemesterID = request.SemesterID,
                });
            }
            _context.CourseAssignments.AddRange(assigments);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminIndex");
        }

        [Authorize(Roles = "Professor, Admin")]
        [ActionName("GivenLoad")]
        public async Task<IActionResult> GivenLoad(int semesterID, int professorID)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User.IsInRole("Professor") && professorID != user.Id) return RedirectToAction("AccessDenied", "Account");
            else
            {


                TeachingRequest request = await _context.TeachingRequests
                    .Include(i => i.SemesterForAssignment)
                    .Include(i => i.ListOfCourses)
                    .ThenInclude(i => i.Course)
                    .Include(i => i.GivenCourses)
                    .Where(i => i.ProfessorID == professorID && i.SemesterID == semesterID)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
                List<GivenLoad> courseLoad = new List<GivenLoad>();
                GivenLoad nova = new GivenLoad();
                foreach (var course in request.ListOfCourses)
                {
                    nova.Course = course.Course;
                    nova.Requested = true;
                    nova.Given = false;
                    nova.Semester = request.SemesterForAssignment;
                    foreach (var given in request.ListOfCourses)
                        if (given.CourseID == course.Course.CourseID)
                        {
                            nova.Given = true;
                            break;
                        }
                    courseLoad.Add(nova);
                }
                bool found;
                foreach (var given in request.GivenCourses)
                {
                    nova.Course = given;
                    nova.Given = true;
                    nova.Requested = false;
                    nova.Semester = request.SemesterForAssignment;
                    found = false;
                    foreach (var course in courseLoad)
                        if (course.Course.CourseID == nova.Course.CourseID) { found = true; break; }
                    if (found == false) courseLoad.Add(nova);
                }
                ViewData["Semester"] = request.SemesterForAssignment.Title;
                ViewData["SemesterID"] = semesterID;
                ViewData["ProfessorID"] = professorID;
                return View(courseLoad);
            }
        }

    }
}

        