using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContosoUniversity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using ContosoUniversity.UniversityFunctionalityModels.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using static ContosoUniversity.Models.SchoolViewModels.ChoseDates;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {
        private SchoolContext _context;

        public ProfessorController(SchoolContext context,
            UserManager<IdentityUser<int>> userManager,
            SignInManager<IdentityUser<int>> signInManager,
            IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }
        private IHostingEnvironment _environment;
        public UserManager<IdentityUser<int>> _userManager { get; private set; }
        public SignInManager<IdentityUser<int>> _signInManager { get; private set; }

        [Authorize(Roles = "Professor")]
        [ActionName("ProfessorIndex")]
        public async Task<IActionResult> ProfessorIndex(int? courseYear, int? courseSemester, int? requestYear, int? requestSemester)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            var viewModel = new ProfessorViews();
            viewModel.Professors = await _context.Professors
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == user.Id);

            //RequestsPart
            PopulateYearAndSemeter(requestYear, requestSemester);
            ViewBag.requestYears = ViewData["years"];
            ViewBag.requestSemesters = ViewData["semesters"];

            if (requestYear == null)
                viewModel.Requests = _context.TeachingRequests.Include(c => c.SemesterForAssignment).Where(s => s.ProfessorID == user.Id && s.SemesterForAssignment.StartYear == (int)ViewData["thisYear"] && (int)s.SemesterForAssignment.Season == (int)ViewData["thisSemester"]).AsNoTracking().ToList();
            else if (requestYear == 0)
                viewModel.Requests = _context.TeachingRequests.Include(c => c.SemesterForAssignment).Where(s => s.ProfessorID == user.Id).AsNoTracking().ToList();
            else if (requestYear != null && requestYear != 0)
                viewModel.Requests = _context.TeachingRequests.Include(c => c.SemesterForAssignment).Where(s => s.ProfessorID == user.Id && s.SemesterForAssignment.StartYear == requestYear && (int)s.SemesterForAssignment.Season == requestSemester).AsNoTracking().ToList();

            //Courses part
            PopulateYearAndSemeter(courseYear, courseSemester);
            ViewBag.courseYears = ViewData["years"];
            ViewBag.courseSemesters = ViewData["semesters"];
            if(courseYear != null && courseYear != 0)
                viewModel.CoursesAssignments = _context.CourseAssignments.Include(c => c.Course).Include(s => s.Semester).AsNoTracking().Where(s => s.ProfessorID == user.Id && s.Semester.StartYear == courseYear && (int)s.Semester.Season == requestSemester).ToList();
            else if(courseYear != null && courseYear == 0)
                viewModel.CoursesAssignments = _context.CourseAssignments.Include(c => c.Course).Include(s => s.Semester).AsNoTracking().Where(s => s.ProfessorID == user.Id).ToList();
            else if (courseYear == null)
            viewModel.CoursesAssignments = _context.CourseAssignments.Include(c => c.Course).Include(s => s.Semester).AsNoTracking().Where(s => s.ProfessorID == user.Id && s.Semester.StartYear == (int)ViewData["thisYear"] && (int)s.Semester.Season == (int)ViewData["thisSemester"]).ToList();
            //viewModel.Requests = _context.TeachingRequests.Include(c => c.SemesterForAssignment).Where(s => s.ProfessorID == user.Id).AsNoTracking().ToList();

            ViewData["ProfessorID"] = user.Id;
            viewModel.Membership = _context.CommitieMembership.Include(i => i.Committee).Where(i => i.ProfessorID == user.Id).ToList();
            var semesters = _context.Semesters.Where(m => m.Open == true);
            ViewBag.Semesters = new SelectList(semesters, "ID", "Title");
            ViewData["Message"] = "Default for professor.";

            return View(viewModel);
        }


        [Authorize(Roles = "Professor")]
        [ActionName("MyAssignment")]
        public async Task<IActionResult> MyAssignment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                CourseAssignment assignment = await _context.CourseAssignments.SingleOrDefaultAsync(i => i.AssignmentID == (int)id);
                return View(assignment);
            }
        }

        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("MyAssignment")]
        public async Task<IActionResult> MyAssignment(CourseAssignment model)
        {
            //var user = await _userManager.FindByNameAsync(User.Identity.Name);
            //if (_context.CourseAssignments.Any(i => i.ProfessorID == user.Id) )
            //    return RedirectToAction("AccessDenied", "Account");
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            else
            {
                CourseAssignment assignment = await _context.CourseAssignments.SingleOrDefaultAsync(i => i.AssignmentID == model.AssignmentID);
                assignment.CourseDescription = model.CourseDescription;
                await _context.SaveChangesAsync();
                return RedirectToAction("ProfessorIndex");
            }
        }

        //This one is out of date due to change of policy :3

        //[Authorize(Roles = "Professor")]
        //public async Task<IActionResult> ProfessorCoursesEdit(int? id)
        //{
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (id != user.Id) return RedirectToAction("AccessDenied", "Account");


        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var professor = await _context.Professors.Include(i => i.Courses)
        //        .ThenInclude(i => i.Course).SingleOrDefaultAsync(m => m.Id == id);
        //    if (professor == null)
        //    {
        //        return NotFound();
        //    }
        //    //var listofenr = new List<Enrollment>();
        //    //foreach (var enrollment in student.Enrollments)
        //    //{
        //    //    listofenr.Add(enrollment);
        //    //}
        //    PopulateEnrolledCourseData(professor);

        //    return View(professor);

        //}

        [Authorize(Roles = "Professor")]
        [ActionName("CoursesToChoose")]
        public async Task<IActionResult> CoursesToChoose(int? requestYear, int? requestSemester,FormCollection formCollection /*,[System.Web.Http.FromUri] string sem*/)
        {
            Semester semesterWeLook = _context.Semesters.Where(i => i.StartYear == requestYear && (int)i.Season == requestSemester).Single();
            if (semesterWeLook.Open != true) { return RedirectToAction("ProfessorIndex"); }
            int sem = semesterWeLook.ID;

            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            var requests = await _context.TeachingRequests.Where(id => id.ProfessorID == user.Id && id.SemesterID == sem).SingleOrDefaultAsync();
            if (requests != null) { return RedirectToAction("ProfessorIndex"); }
            else
            {
                TeachingRequest request = new TeachingRequest()
                {
                    ProfessorID = user.Id,
                };
                if (HttpContext.Session.GetObjectFromJson<TeachingRequest>("request") == null)
                {
                    var semesters = _context.Semesters.OrderBy(m => m.Season).Where(m => m.Open == true).AsNoTracking();
                    ViewBag.Semesters = new SelectList(semesters, "ID", "Season", sem);
                    request.SemesterID = sem;
                    //request.SemesterForAssignment = await _context.Semesters.Where(i => i.ID == Int32.Parse(sem)).AsNoTracking().SingleOrDefaultAsync();
                    HttpContext.Session.SetObjectAsJson("request", request);
                }
                else
                {
                    request = HttpContext.Session.GetObjectFromJson<TeachingRequest>("request");
                    var semesters = _context.Semesters.OrderBy(m => m.Season).Where(m => m.Open == true).AsNoTracking();
                    ViewBag.Semesters = new SelectList(semesters, "ID", "Season", request.SemesterID);
                }
                CoursesToChoose courses = new CoursesToChoose();
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
                courses.Courses = somelist;


                return View(courses);
            }
        }

        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("SubmitRequest")]
        public async Task<IActionResult> CoursesToChoose(CoursesToChoose model, FormCollection formCollection)
        {
            foreach (ChoosenCourse item in model.Courses)
            {
                item.SelectedCourses = await _context.Courses.Where(i => i.CourseID == item.SelectedCourses.CourseID).AsNoTracking().SingleOrDefaultAsync();
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var request = HttpContext.Session.GetObjectFromJson<TeachingRequest>("request");
                if (_context.TeachingRequests.Any(i => i.ProfessorID == user.Id && i.SemesterID == request.SemesterID))
                    return RedirectToAction("AccessDenied", "Account");
                //Honestly, I have no clue what I've done here. 
                //I better check this later, browser can make unexpected things here.
                //Whole HttpContext usage unnessecery. Or maybe not. 

                request.ListOfCourses = new List<CoursePreference>();
                foreach (ChoosenCourse item in model.Courses)
                    if (item.Checked == true)
                    {
                        //somelist.Add(item.SelectedCourses);
                        _context.Courses.Attach(item.SelectedCourses);
                        request.ListOfCourses.Add(new CoursePreference { Choice = item.Choice, CourseID = item.SelectedCourses.CourseID, Course = item.SelectedCourses });
                    }
                request.ProfessorEntity = await _context.Professors.Include(i => i.Employment).Where(i => i.Id == request.ProfessorID).SingleOrDefaultAsync();
                _context.Professors.Attach(request.ProfessorEntity);
                //request.ListOfCourses = somelist;
                _context.Add(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ProfessorIndex");
        }

        [Authorize(Roles = "Professor")]
        [ActionName("MyRequest")]
        public async Task<IActionResult> MyRequest(int SemesterID, int ProfessorID)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (ProfessorID != user.Id) return RedirectToAction("AccessDenied", "Account");
            else
            {
                TeachingRequest request = await _context.TeachingRequests.Include(i => i.ListOfCourses).ThenInclude(i => i.Course).Where(i => i.ProfessorID == ProfessorID && i.SemesterID == SemesterID).AsNoTracking().SingleOrDefaultAsync();
                return View(request);
            }
        }
        [Authorize(Roles = "Professor")]
        [ActionName("GivenLoad")]
        public async Task<IActionResult> GivenLoad(int SemesterID, int ProfessorID)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (ProfessorID != user.Id) return RedirectToAction("AccessDenied", "Account");
            else
            {
                
                    
                TeachingRequest request = await _context.TeachingRequests
                    .Include(i => i.SemesterForAssignment)
                    .Include(i => i.ListOfCourses)
                    .ThenInclude(i => i.Course)
                    .Include(i => i.GivenCourses)
                    .Where(i => i.ProfessorID == ProfessorID && i.SemesterID == SemesterID)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
                List<GivenLoad> courseLoad = new List<GivenLoad>();
                GivenLoad nova = new GivenLoad();
                foreach (var course in request.ListOfCourses) {
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
                return View(courseLoad);
            }
        }
        private void PopulateEnrolledCourseData(Professor professor)
        {
            var allCourses = _context.Courses;
            if (professor.Courses == null)
                professor.Courses = new List<CourseAssignment>();
            var professorCourses = new HashSet<int>(professor.Courses.Select(c => c.Course.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = professorCourses.Contains(course.CourseID),

                });
            }
            ViewData["Courses"] = viewModel;
        }
        //[Authorize(Roles = "Professor")]
        //[HttpGet, ActionName("ListCommittees")]
        //public async Task<IActionResult> ListCommittees()
        //{
        //    IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    Professor professor = await _context.Professors.AsNoTracking().SingleOrDefaultAsync(i => i.Id == user.Id);
        //    IEnumerable<CommitieMembership> memrbership = await _context.CommitieMembership.Where(i => i.ProfessorID == user.Id).ToListAsync();
        //    IEnumerable<Committee> committees = await _context.Committees.AsNoTracking().ToListAsync();
        //    ProfCommView model = new ProfCommView()
        //    {
        //        Professor = professor,
        //        Committees = committees,
        //        Membership = memrbership,
        //    };
        //    return View(model);
        //}
        //[Authorize(Roles = "Professor")]
        //[HttpPost, ActionName("ApplyToCommittee")]
        //public async Task<IActionResult> ApplyToCommittee()
        //{
        //    IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    int commId = int.Parse(Request.Form["id"]);
        //    CommitieMembership member = new CommitieMembership()
        //    {
        //        Request = true,
        //        ProfessorID = user.Id,
        //        CommitteeID = commId,
        //        EstimatedEndDate = _context.Semesters.AsNoTracking().SingleOrDefault(i => i.current == true).EndDate,
        //    };
        //    _context.CommitieMembership.Add(member);
        //    await _context.SaveChangesAsync();
        //    return Json("Success");
        //}
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("RemoveFromCommittee")]
        public async Task<IActionResult> RemoveFromCommittee(int? ProfID, int? CommID)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            var membership = _context.CommitieMembership.Where(i => i.CommitteeID == CommID && i.ProfessorID == ProfID && i.FinishedWork != true).SingleOrDefault();
            membership.EndDate = DateTime.Now;
            membership.FinishedWork = true;
            if (membership.Chair == true)
            {
                membership.Committee.ProfessorID = null;
            }
            //_context.CommitieMembership.Remove(membership);
            await _context.SaveChangesAsync();
            return Json("Success");
        }
        [Authorize(Roles = "Professor")]
        [ActionName("LeaveCommittee")]
        public async Task<IActionResult> LeaveCommittee(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            //int commId = int.Parse(Request.Form["id"]);
            CommitieMembership request = await _context.CommitieMembership
                .SingleOrDefaultAsync(i => (i.CommitteeID == id) && (i.ProfessorID == user.Id) && (i.FinishedWork != true));
            if (request == null)
            {
                return NotFound();
            }
            request.FinishedWork = true;
            request.EndDate = DateTime.Today;
            await _context.SaveChangesAsync();
            return RedirectToAction("ProfessorIndex");
        }

        [Authorize(Roles = "Professor")]
        [HttpGet, ActionName("MyCommittee")]
        public async Task<IActionResult> MyCommittee(int? id, int? year, int? membership)
        {
            if (id == null)
            {
                return NotFound();
            }

            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewData["ProfessorID"] = user.Id;
            Committee committee = await _context.Committees
                .Include(i => i.Chair)
                .Include(i => i.CommitieMembers)
                .ThenInclude(i => i.Professor)
                .ThenInclude(i => i.OfficeAssignment)
                .Include(i => i.CommitieMembers)
                .ThenInclude(i => i.Professor)
                .ThenInclude(i => i.Employment)
                .ThenInclude(i => i.Department)
                .Include(i => i.Meetings)
                .ThenInclude(i => i.Suggestions)
                .ThenInclude(i => i.Checkers)
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.CommitteeID == id);

            if (committee.CommitieMembers.Any(i => i.FinishedWork != true && i.ProfessorID == user.Id))
            {
                PopulateYearAndSemeter(year, null);

                ViewBag.Years = ViewData["years"] ;
                if (year != null) ViewData["year"] = year;

                IEnumerable<CommitieMembership> members = new List<CommitieMembership>();
                IEnumerable<Meetings> meetings = new List<Meetings>();
                if (year == null)
                {
                    if (membership == 2)
                        members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == DateTime.Today.Year).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    else if (membership == 1)
                        members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == DateTime.Today.Year && i.FinishedWork == true).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    else if (membership == 0 || membership == null)
                        members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == DateTime.Today.Year && i.FinishedWork == false).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    meetings = committee.Meetings.Where(i => i.OpenDate.Year == DateTime.Today.Year).ToList();
                }
                else if (year != 0)
                {
                    if (membership == 2)
                        members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == year).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    if (membership == 1)
                        members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == year && i.FinishedWork == true).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    if (membership == 0 || membership == null)
                        members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == year && i.FinishedWork == false).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    meetings = committee.Meetings.Where(i => i.OpenDate.Year == year).ToList();
                }
                else
                {
                    if (membership == 2)
                        members = committee.CommitieMembers.OrderByDescending(i => i.DateOfEnrollment).ToList();
                    else if (membership == 1)
                        members = committee.CommitieMembers.Where(i => i.FinishedWork == true).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    else if (membership == 0 || membership == null)
                        members = committee.CommitieMembers.Where(i => i.FinishedWork == false).OrderByDescending(i => i.DateOfEnrollment).ToList();
                    meetings = committee.Meetings.ToList();
                }
                if(membership != null)
                    ViewData["membership"] = membership;
                else
                    ViewData["membership"] = 0;

                MyCommittee MyCommittee = new MyCommittee()
                {
                    Committee = committee,
                    Members = members,
                    Meetings = meetings,
                };
                return View(MyCommittee);
            }
            else return RedirectToAction("AccessDenied", "Account");
        }
        [Authorize(Roles = "Professor")]
        [ActionName("AddMembers")]
        public async Task<IActionResult> AddMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                int CommID = (int)id;
                var profs = await _context.Professors.Include(i => i.Employment).ThenInclude(i => i.Department).ThenInclude(i => i.Faculty).Include(i => i.Commities).AsNoTracking().ToListAsync();
                for (int i = 0; i < profs.Count; i++)
                {
                    var prof = profs[i];
                    foreach (var committie in prof.Commities)
                    {
                        if (committie.CommitteeID == CommID && committie.FinishedWork != true)
                        {
                            profs.Remove(prof);
                            i--;
                            break;
                        }
                    }
                }
                CommitteeInvitation InviteList = new CommitteeInvitation
                {
                    Professors = profs,
                    CommitteeID = CommID,
                    Memberships = await _context.CommitieMembership.Where(i => i.CommitteeID == CommID).ToListAsync(),
                };
                return View(InviteList);
            }
        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("Invite")]
        public async Task<IActionResult> Invite()
        {
            int profId = int.Parse(Request.Form["ProfID"]);
            int commId = int.Parse(Request.Form["CommID"]);
            CommitieMembership member = new CommitieMembership()
            {
                Chair = false,
                ProfessorID = profId,
                CommitteeID = commId,
                FinishedWork = false,
                DateOfEnrollment = DateTime.Today.Date,
                EstimatedEndDate = new DateTime(_context.Semesters.AsNoTracking().SingleOrDefault(i => i.Current == true).EndYear, 5, 1),
            };
            _context.CommitieMembership.Add(member);
            await _context.SaveChangesAsync();
            return Json("Success");
        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("Leave")]
        public async Task<IActionResult> Leave(int? id)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (id == null)
            {
                return NotFound();
            }
            var membership = _context.CommitieMembership.Single(i => i.CommitteeID == id && i.ProfessorID == user.Id);
            _context.CommitieMembership.Remove(membership);
            return RedirectToAction("ProfessorIndex");
        }
        [Authorize(Roles = "Professor")]
        [HttpGet, ActionName("SetMeeting")]
        public async Task<IActionResult> SetMeeting(int? id)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (id == null)
            {
                return NotFound();
            }
            Committee committee = _context.Committees.AsNoTracking().Single(i => i.CommitteeID == id);
            ViewData["CommitteeID"] = committee.CommitteeID;
            if (committee.ProfessorID != user.Id)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View();

        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("SetMeeting")]
        public async Task<IActionResult> SetMeeting(Meetings model, string CommitteeID, ICollection<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                model.Suggestions = new List<DatesSuggestion>();
                DatesSuggestion date;
                DateTime datetime;
                for (int i=0; i<5; i++)
                {
                    var suggestion = Request.Form["Suggestions[" + i + "]"];
                    if (suggestion != "")
                    {
                        datetime = Convert.ToDateTime(Request.Form["Suggestions[" + i + "]"]);
                        date = new DatesSuggestion()
                        {
                            Value = datetime,
                        };
                        model.Suggestions.Add(date);
                    }
                }
                model.CommitteeID = int.Parse(CommitteeID);
                model.FinalDate = false;
                model.OpenDate = DateTime.Today;
                _context.Meetings.Add(model);
                await _context.SaveChangesAsync();
                Committee committee = await _context.Committees.SingleAsync(i => i.CommitteeID == model.CommitteeID);
                return RedirectToAction("MyCommittee", new { id = committee.CommitteeID });
            }
            return View(model);
        }
        [Authorize(Roles = "Professor")]
        [HttpGet, ActionName("EditMeeting")]
        public async Task<IActionResult> EditMeeting(int? comID, int? mtnID)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (comID == null || mtnID == null)
            {
                return NotFound();
            }
            Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.CommitteeID == comID && i.MeetingID == mtnID);
            ViewData["CommitteeID"] = meeting.CommitteeID;
            if (meeting.Committee.ProfessorID != user.Id)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View(meeting);

        }
        [Authorize(Roles = "Professor")]
        [ActionName("ViewMeeting")]
        public async Task<IActionResult> ViewMeeting(int? comID, int? mtnID)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (comID == null || mtnID == null)
            {
                return NotFound();
            }
            MeetingView view = new MeetingView();
            view.Meeting = _context.Meetings.Include(i => i.Files).Include(i => i.Comments).ThenInclude(i => i.Files).Single(i => i.CommitteeID == comID && i.MeetingID == mtnID);
            view.PublicComments = view.Meeting.Comments.Where(i => i.Private == false).ToList();
            view.PrivateComments = view.Meeting.Comments.Where(i => i.Private == true && i.ProfessorID == user.Id).ToList();

            var PublicFiles = view.Meeting.Files.Where(i => i.OwnerID == mtnID && i.Owned == Ownership.meetingPub).ToList();
            List<FilesAssosiation> publicFiles = new List<FilesAssosiation>();
            foreach (var file in PublicFiles)
                publicFiles.Add(new FilesAssosiation()
                {
                    File = file,
                    Author = _context.Professors.Single(i => i.Id == file.OwnerID).FullName,
                });
            view.PublicFiles = publicFiles;

            var PrivateFiles = view.Meeting.Files.Where(i => i.OwnerID == user.Id && i.Owned == Ownership.meetingPriv).ToList();
            List<FilesAssosiation> privateFiles = new List<FilesAssosiation>();
            foreach (var file in PrivateFiles)
                privateFiles.Add(new FilesAssosiation()
                {
                    File = file,
                    Author = _context.Professors.Single(i => i.Id == file.OwnerID).FullName,
                });
            view.PrivateFiles = privateFiles;

            ViewData["CommitteeID"] = view.Meeting.CommitteeID;
            ViewData["ProfessorID"] = user.Id;
            return View(view);
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("EditMeeting")]
        public async Task<IActionResult> EditMeeting(Meetings model, string CommitteeID)
        {
            var meetingToUpdate = _context.Meetings.Where(i => i.CommitteeID == model.CommitteeID && i.MeetingID == model.MeetingID).Single();
            if (ModelState.IsValid)
            {
                model.CommitteeID = int.Parse(CommitteeID);
                await TryUpdateModelAsync<Meetings>(
                meetingToUpdate,
                "",
                s => s.Location, s => s.Title, s => s.Date);

                await _context.SaveChangesAsync();
                Committee committee = await _context.Committees.SingleAsync(i => i.CommitteeID == model.CommitteeID);
                return RedirectToAction("MyCommittee", new { id = committee.CommitteeID });
            }
            return View(model);
        }
        //[Authorize(Roles = "Professor")]
        //[HttpGet, ActionName("CommentAdd")]
        //public async Task<IActionResult> CommentAdd(int? id, int? comID)
        //{
        //    IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    Professor prof = _context.Professors.Single(i => i.Id == user.Id);
        //    if (id == null || comID == null)
        //    {
        //        return NotFound();
        //    }
        //    MeetingComment comment = new MeetingComment()
        //    {
        //        MeetingID = (int)id,
        //        ProfessorID = user.Id,
        //        ProfessorName = prof.FullName,
        //        CommitteeID = (int)comID,
        //    };
            
        //    return View(comment);

        //}
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("CommentAdd")]
        public async Task<IActionResult> CommentAdd(MeetingComment model/*, ICollection<IFormFile> files*/)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
                Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);


                if (meeting.Files == null) model.Files = new List<FileBase>();
                model.ProfessorName = _context.Professors.Single(i => i.Id == user.Id).FullName;
                model.DateStamp = DateTime.Now;
                //var uploads = Path.Combine(_environment.WebRootPath, "uploads/Committees");
                //foreach (var file in files)
                //{
                //    if (file.Length > 0)
                //    {
                //        string name = file.FileName;
                //        MD5 hasher = MD5.Create();
                //        DateTime now = new DateTime();
                //        now = DateTime.Now;
                //        string fileName = GetMd5Hash(hasher, now.ToString()) + Path.GetExtension(file.FileName);

                //        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                //        {
                //            await file.CopyToAsync(fileStream);
                //        }
                //        FileBase fileToSave = new FileBase()
                //        {
                //            Location = "/uploads/Committees/" + fileName,
                //            Owned = Ownership.meetingPub,
                //            OwnerID = meeting.MeetingID,
                //            Added = DateTime.Now,
                //            ViewTitle = name,
                //        };
                //        model.Files.Add(fileToSave);
                //    }
                //}
                _context.MeetComments.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });
            }
            return View(model);
        }

        //[Authorize(Roles = "Professor")]
        //[HttpGet, ActionName("FileAdd")]
        //public async Task<IActionResult> FileAdd(int? id, int? comID)
        //{
        //    IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    Professor prof = _context.Professors.Single(i => i.Id == user.Id);
        //    if (id == null || comID == null)
        //    {
        //        return NotFound();
        //    }
        //    MeetingComment comment = new MeetingComment()
        //    {
        //        MeetingID = (int)id,
        //        ProfessorID = user.Id,
        //        ProfessorName = prof.FullName,
        //        CommitteeID = (int)comID,
        //    };

        //    return View(comment);

        //}

        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("FileAdd")]
        public async Task<IActionResult> FileAdd(MeetingComment model, ICollection<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);

                if (files == null) return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });
                if (meeting.Files == null) meeting.Files = new List<FileBase>();

                var uploads = Path.Combine(_environment.WebRootPath, "uploads/Committees");
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string name = file.FileName;
                        MD5 hasher = MD5.Create();
                        DateTime now = new DateTime();
                        now = DateTime.Now;
                        string fileName = GetMd5Hash(hasher, now.ToString()) + Path.GetExtension(file.FileName);

                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        if (model.Private == true)
                        {
                            FileBase fileToSave = new FileBase()
                            {
                                Location = "/uploads/Committees/" + fileName,
                                Owned = Ownership.meetingPriv,
                                OwnerID = model.ProfessorID,
                                Added = DateTime.Now,
                                ViewTitle = name,
                            };
                            meeting.Files.Add(fileToSave);
                        }
                        else
                        {
                            FileBase fileToSave = new FileBase()
                            {
                                Location = "/uploads/Committees/" + fileName,
                                Owned = Ownership.meetingPub,
                                OwnerID = model.ProfessorID,
                                Added = DateTime.Now,
                                ViewTitle = name,
                            };
                            meeting.Files.Add(fileToSave);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });



                }
                return View(model);
            }
            return RedirectToAction("ProfessorIndex");
        }

        [Authorize(Roles = "Professor")]
        [HttpGet, ActionName("CommentEdit")]
        public async Task<IActionResult> CommentEdit(int? id, int? comID, int? priva)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            Professor prof = _context.Professors.Single(i => i.Id == user.Id);
            if (id == null || priva == null || comID == null)
            {
                return NotFound();
            }
            bool privatni = false;
            if (priva == 1)
            {
                privatni = false;
            }
            else if (priva == 2)
            {
                privatni = true;
            }
            MeetingComment comment = _context.MeetComments.Include(i => i.Files).Single(i => i.Private == privatni && i.ProfessorID == user.Id && i.MeetingID == id);
            ViewBag.Files = comment.Files;
            return View(comment);

        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("RemoveFiles")]
        public async Task<IActionResult> RemoveFiles(int? fileID, int? commID, int? profID, int? priva)
        {
            List<FileBase> files = new List<FileBase>();
            bool privatni = false;
            if (priva == 1)
            {
                privatni = true;
            }
            else if (priva == 2)
            {
                privatni = false;
            }
            MeetingComment comment = _context.MeetComments.Include(i => i.Files).Where(i => i.CommitteeID == commID && i.Private == privatni && i.ProfessorID == profID).Single();
            FileBase file = comment.Files.Single(i => i.FileBaseID == fileID);
            comment.Files.Remove(file);
            _context.SaveChanges();
            return Json("success");
        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("CommentEdit")]
        public async Task<IActionResult> CommentEdit(MeetingComment model, ICollection<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);


                if (meeting.Files == null) model.Files = new List<FileBase>();


                var uploads = Path.Combine(_environment.WebRootPath, "uploads/Committees");
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        string name = file.FileName;
                        MD5 hasher = MD5.Create();
                        DateTime now = new DateTime();
                        now = DateTime.Now;
                        string fileName = GetMd5Hash(hasher, now.ToString()) + Path.GetExtension(file.FileName);

                        using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        FileBase fileToSave = new FileBase()
                        {
                            Location = "/uploads/Committees/" + fileName,
                            Owned = Ownership.meetingPub,
                            OwnerID = meeting.MeetingID,
                            Added = DateTime.Now,
                            ViewTitle = name,
                        };
                        model.Files.Add(fileToSave);
                    }
                }
                MeetingComment MeetingComm = _context.MeetComments.Include(i => i.Files).Single(i => i.ProfessorID == model.ProfessorID && i.CommitteeID == model.CommitteeID && model.MeetingID == i.MeetingID && i.Private == model.Private);
                await TryUpdateModelAsync<MeetingComment>(
                                MeetingComm,
                                "",
                                i => i.Comment, i => i.Files);
                foreach (var item in model.Files)
                    MeetingComm.Files.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });



            }
            return View(model);

        }


        [Authorize(Roles = "Professor")]
        [ActionName("SuggestDate")]
        public async Task<IActionResult> SuggestDate(int? id)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (id == null)
            {
                return NotFound();
            }

            ChoseDates suggestion = new ChoseDates()
            {
                MeetingID = (int)id,
                ProfessorID = user.Id,
                Dates = new List<DateChoice>(),
            };
            List<DatesSuggestion> dates = _context.DatesSuggestion.Where(i => i.MeetingID == (int)id).ToList();
            DateChoice choice;
            foreach (var date in dates)
            {
                choice = new DateChoice()
                {
                    date = date,
                };
                suggestion.Dates.Add(choice);
            }
            ViewData["CommitteeID"] = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == (int)id).Committee.CommitteeID;
            return View(suggestion);
        }
        [Authorize(Roles = "Professor")]
        [ActionName("Suggestions")]
        public async Task<IActionResult> Suggestions(int? id)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (id == null)
            {
                return NotFound();
            }
            ChooseMeetingDate set = new ChooseMeetingDate();
            List<DatesSuggestion> suggestions = await _context.DatesSuggestion.Include(i=> i.Checkers).Where(i => i.MeetingID == id).ToListAsync();
            set.Members = _context.Meetings.Include(c => c.Committee)
                .ThenInclude(c => c.CommitieMembers)
                .ThenInclude(c => c.Professor)
                .Single(i => i.MeetingID == id)
                .Committee.CommitieMembers
                .Where(i => i.FinishedWork == false && i.Professor != i.Committee.Chair)
                .ToList();
            set.Dates = _context.DatesSuggestion.Where(i => i.MeetingID == id).ToList();
            ViewData["MeetingID"] = id;
            ViewData["CommitteeID"] = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == (int)id).Committee.CommitteeID;
            return View(set);
        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("SuggestDate")]
        public async Task<IActionResult> SuggestDate(ChoseDates model)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            Professor professor = _context.Professors.Single(i => i.Id == user.Id);
            //var membership =  _context.Meetings.Single(i => i.MeetingID == model.MeetingID).Committee.CommitieMembers.Single(d => d.ProfessorID == user.Id);
            //if (membership == null)
            //{
            //    return RedirectToAction("AccessDenied", "Account");
            //}
            if (ModelState.IsValid)
            {
                //var Meeting = _context.Meetings.Include(i => i.Suggestions).Single(i => i.MeetingID == model.MeetingID);
                DatesSuggestion suggestion;
                foreach (var date in model.Dates)
                    if(date.choice == true)
                    {
                        suggestion = _context.DatesSuggestion.Include(i=>i.Checkers).Single(i => i.SuggestionID == date.date.SuggestionID);
                        suggestion.Checkers.Add(professor);
                        //Meeting.Suggestions.Single(i => i.SuggestionID == date.date.SuggestionID).Checkers.Add(professor);
                    }
                await _context.SaveChangesAsync();
                Committee committee = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID).Committee;
                return RedirectToAction("MyCommittee", new { id = committee.CommitteeID });
            }
            return View(model);
        }
        [Authorize(Roles = "Professor")]
        [HttpPost, ActionName("SetMeetingDate")]
        public async Task<IActionResult> SetMeetingDate(string CommitteeID, string Date )
        {
            DateTime date = Convert.ToDateTime(Request.Form["Date"]);
            int meetingID = int.Parse(Request.Form["CommitteeID"]);
            Meetings meeting = _context.Meetings.SingleOrDefault(i => i.MeetingID == meetingID);
            meeting.Date = date;
            meeting.FinalDate = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("MyCommittee", new { id = meeting.CommitteeID });
        }
        //// POST: Students/Edit/5
        ////custom method to add/remove MYcourses (student)
        //[HttpPost, ActionName("MyCourse")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MyCourse(Enrollment myenrollment)
        //{

        //    Enrollment enrollmentToUpdate = await _context.Enrollments.FirstOrDefaultAsync(i => i.EnrollmentID == myenrollment.EnrollmentID);
        //    enrollmentToUpdate.Notes = myenrollment.Notes;
        //    enrollmentToUpdate.Grade = myenrollment.Grade;
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
        //    return RedirectToAction("StudentIndex");
        //}
        private Task<IdentityUser<int>> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        private void PopulateYearAndSemeter(int? year, int? semester)
        {
            List<SelectListItem> years = new List<SelectListItem>();
            int currentYear;
            if (DateTime.Today.Month < 5)
                 currentYear = DateTime.Today.Year - 1;
            else currentYear = DateTime.Today.Year;
            ViewData["thisYear"] = currentYear;

            if (year == null)
            {
                for (int i = 2015; i < currentYear + 2; i++)
                {
                    if (currentYear != i)
                    {
                        years.Add(new SelectListItem { Text = i + " - " + (i + 1), Value = i.ToString() });
                    }
                    else
                    {
                        years.Add(new SelectListItem { Text = i + " - " + (i + 1), Value = i.ToString(), Selected = true });
                    }
                }
            }
            else
            {
                for (int i = 2015; i < currentYear + 2; i++)
                {
                    if (year != i)
                    {
                        years.Add(new SelectListItem { Text = i + " - " + (i + 1), Value = i.ToString() });
                    }
                    else
                    {
                        years.Add(new SelectListItem { Text = i + " - " + (i + 1), Value = i.ToString(), Selected = true });
                    }
                }
            }
            if (year != 0)
                years.Add(new SelectListItem { Text = "All Time", Value = "0" });
            else
                years.Add(new SelectListItem { Text = "All Time", Value = "0", Selected = true });

            ViewData["years"] = years;

            List<SelectListItem> semesters = new List<SelectListItem>()
            {

                new SelectListItem { Text = "Summer First", Value = "1" },
                new SelectListItem { Text = "Summer Second", Value = "2" },
                new SelectListItem { Text = "Autumn", Value = "3" },
                new SelectListItem { Text = "Winter", Value = "4" },
            };
            var currentSemester = _context.Semesters.Where(i => i.Current).Single();
            ViewData["thisSemester"] = currentSemester.Season;
            if (semester == null)
            {
                semesters.Single(i => int.Parse(i.Value) == (int)currentSemester.Season).Selected = true;
            }
            else
            {
                semesters[(int)semester-1].Selected = true;
            }
            ViewData["semesters"] = semesters;
        }
    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

}