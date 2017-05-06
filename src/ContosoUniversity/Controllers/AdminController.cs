using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContosoUniversity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using ContosoUniversity.Models.Entities;

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private SchoolContext _context;

        public AdminController(SchoolContext context,
            UserManager<IdentityUser<int>> userManager,
            SignInManager<IdentityUser<int>> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public UserManager<IdentityUser<int>> _userManager { get; private set; }
        public SignInManager<IdentityUser<int>> _signInManager { get; private set; }

        public async Task<IActionResult> AdminIndex()
        {
            AdminIndex viewModel = new AdminIndex();
            viewModel.NewStudents = await _context.Students.Include(p => p.Program).Where(c => c.Approved == false).ToListAsync();
            viewModel.Requests = await _context.TeachingRequests.Include(i => i.ProfessorEntity).Include(c => c.SemesterForAssignment).Where(c => c.Approved == false && c.SemesterForAssignment.Open == true).ToListAsync();
            viewModel.Commities = await _context.Committees.Include(c => c.Chair).Include(c => c.CommitieMembers).ToListAsync();
            return View(viewModel);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("StudentApproval")]
        public async Task<IActionResult> StudentApproval()
        {
            int id = int.Parse(Request.Form["ID"]);
            Student user = await _context.Students.Where(i => i.Id == id).SingleOrDefaultAsync();
            user.Approved = true;
            //IdentityUser<int> student = await _userManager.FindByNameAsync(user.UserName);
            await _userManager.AddToRoleAsync(user, "Student");
            await _context.SaveChangesAsync();
            return Json("Nice!");
        }

        // Committie creation and management is now in Committee controller

        //[Authorize(Roles = "Admin")]
        //[ActionName("CreateCommitie")]
        //public async Task<IActionResult> CreateCommitie()
        //{
        //    ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "FullName");
        //    ViewData["Professor"] = await _context.Professors.AsNoTracking().ToListAsync();
        //    ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i=> i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
        //    ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");
        //    Committee committee = new Committee();
        //    //committee.SemesterID = _context.Semesters.Where(i => i.current == true).SingleOrDefault().ID;
        //    return View(committee);
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("SubmitCommitie")]
        //public async Task<IActionResult> CreateCommitie(Committee model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.Level == Level.Department) model.FacultyID = null;
        //        else if (model.Level == Level.Faculty) model.DepartmentID = null;
        //        else if (model.Level == Level.University) { model.FacultyID = null;  model.DepartmentID = null; }
        //        //model.StartDate = DateTime.Today.Date;
        //        if (model.ProfessorID != null)
        //        {
        //            DateTime end = new DateTime();
        //            if (DateTime.Today < new DateTime(DateTime.Today.Year, 6, 1)) end = new DateTime(DateTime.Today.Year, 6, 1);
        //            else end = new DateTime(DateTime.Today.Year + 1, 6, 1);
        //            CommitieMembership member = new CommitieMembership()
        //            {
        //                Chair = true,
        //                ProfessorID = (int)model.ProfessorID,
        //                DateOfEnrollment = DateTime.Now,
        //                EstimatedEndDate = end,
        //            };
        //            model.CommitieMembers = new List<CommitieMembership>()
        //            {
        //                member,
        //            };
        //        }
        //        _context.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("AdminIndex");
        //    }
        //    return View(model);
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("UpdateCommittee")]
        //public async Task<IActionResult> UpdateCommittee(Committee model)
        //{
        //    if (model.Level == Level.Department) model.FacultyID = null;
        //    else if (model.Level == Level.Faculty) model.DepartmentID = null;
        //    else if (model.Level == Level.University) { model.FacultyID = null; model.DepartmentID = null; }

        //    var committeeToUpdate = await _context.Committees.Include(i => i.CommitieMembers).SingleOrDefaultAsync(s => s.CommitteeID == model.CommitteeID);
        //    if (committeeToUpdate.ProfessorID != model.ProfessorID)
        //    {
        //        bool found = false;
        //        foreach (var member in committeeToUpdate.CommitieMembers)
        //        {
        //            if (member.ProfessorID == model.ProfessorID && member.Chair == false)
        //            {
        //                member.Chair = true;
        //                found = true;
        //            }
        //            else if (member.ProfessorID != model.ProfessorID && member.Chair == true)
        //            {
        //                member.Chair = false;
        //            }
        //            else if (member.ProfessorID == model.ProfessorID && member.Chair == true)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //        if (found == false)
        //        {
        //            DateTime end = new DateTime();
        //            if (DateTime.Today < new DateTime(DateTime.Today.Year, 6, 1)) end = new DateTime(DateTime.Today.Year, 6, 1);
        //            else end = new DateTime(DateTime.Today.Year+1, 6, 1);
        //            CommitieMembership member = new CommitieMembership()
        //            {
        //                Chair = true,
        //                ProfessorID = (int)model.ProfessorID,
        //                CommitteeID = model.CommitteeID,
        //                DateOfEnrollment = DateTime.Now,
        //                EstimatedEndDate = end,
        //            };
        //            _context.CommitieMembership.Add(member);
        //        }
                
        //    }
        //    if (await TryUpdateModelAsync<Committee>(
        //        committeeToUpdate,
        //        "",
        //        s => s.CommitteeID, s => s.ProfessorID, s => s.FacultyID, s => s.Title, s => s.Level))
        //    {

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("AdminIndex");
        //    }
        //    else return RedirectToAction("ManageCommitie", new { id = model.CommitteeID });

        //    //if (ModelState.IsValid)
        //    //{
        //    //    if (model.Level == Level.Department) model.FacultyID = null;
        //    //    else if (model.Level == Level.Faculty) model.DepartmentID = null;
        //    //    else if (model.Level == Level.University) { model.FacultyID = null; model.DepartmentID = null; }
        //    //    _context.Committees.Attach(model);
        //    //    await _context.SaveChangesAsync();
        //    //    return RedirectToAction("AdminIndex");
        //    //}
        //    //return View(model);
        //}

        //[Authorize(Roles = "Admin")]
        //[ActionName("ManageCommitie")]
        //public async Task<IActionResult> ManageCommitie(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Committee committee = await _context.Committees
        //        .Include(i => i.CommitieMembers)
        //        .ThenInclude(i => i.Professor)
        //        .ThenInclude(i => i.OfficeAssignment)
        //        .SingleOrDefaultAsync(i => i.CommitteeID == id);
        //    ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "FullName", committee.ProfessorID);
        //    if (committee.Level == Level.Department)
        //    {
        //        ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", committee.DepartmentID, "Faculty.Name");
        //        ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");

        //    }
        //    else if (committee.Level == Level.Faculty)
        //    {
        //        ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
        //        ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name", committee.FacultyID);
        //    }else if (committee.Level == Level.University)
        //    {
        //        ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
        //        ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");
        //    }
        //    ViewData["Professor"] = await _context.Professors.AsNoTracking().ToListAsync();
        //    return View(committee);
        //}

        //[Authorize(Roles = "Admin")]
        //[ActionName("ViewCommittee")]
        //public async Task<IActionResult> ViewCommittee(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    Committee committee = await _context.Committees
        //        .Include(i => i.Department)
        //        .Include(i => i.Faculty)
        //        .Include(i => i.CommitieMembers)
        //        .ThenInclude(i => i.Professor)
        //        .ThenInclude(i => i.OfficeAssignment)
        //        .Include(i => i.CommitieMembers)
        //        .ThenInclude(i => i.Professor)
        //        .ThenInclude(i => i.Employment)
        //        .ThenInclude(i => i.Department)
        //        .AsNoTracking()
        //        .SingleOrDefaultAsync(i => i.CommitteeID == id);
        //    IEnumerable<Meetings> meetings = new List<Meetings>();
        //    IEnumerable<CommitieMembership> members = new List<CommitieMembership>();

        //    if (committee.CommitieMembers != null)
        //    {
        //        members = committee.CommitieMembers.ToList();
        //    }

        //    if (committee.Meetings != null)
        //    {
        //        meetings = committee.Meetings.ToList();
        //    }


        //    MyCommittee myCommittee = new MyCommittee()
        //    {
        //        Committee = committee,
        //        Members = members,
        //        Meetings = meetings,
        //    };
        //    return View(myCommittee);
        //}

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

        //[Authorize(Roles = "Admin")]
        //[ActionName("AddMembers")]
        //public async Task<IActionResult> AddMembers(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        int CommID = (int)id;
        //        var profs = await _context.Professors.Include(i => i.Employment).ThenInclude(i => i.Department).ThenInclude(i => i.Faculty).Include(i => i.Commities).AsNoTracking().ToListAsync();
        //        for (int i = 0; i< profs.Count; i++)
        //        {
        //            var prof = profs[i];
        //            foreach (var committie in prof.Commities)
        //            {
        //                if (committie.CommitteeID == CommID && committie.FinishedWork != true)
        //                {
        //                    profs.Remove(prof);
        //                    i--;
        //                    break;
        //                }
        //            }
        //        }
        //        CommitteeInvitation InviteList = new CommitteeInvitation
        //        {
        //            Professors = profs,
        //            CommitteeID = CommID,
        //        };
        //        return View(InviteList);
        //    }
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("Invite")]
        //public async Task<IActionResult> Invite()
        //{
        //    int profId = int.Parse(Request.Form["ProfID"]);
        //    int commId = int.Parse(Request.Form["CommID"]);
        //    CommitieMembership member = new CommitieMembership()
        //    {
        //        Chair = false,
        //        ProfessorID = profId,
        //        CommitteeID = commId,
        //        DateOfEnrollment = DateTime.Now,
        //        EstimatedEndDate = new DateTime(_context.Semesters.AsNoTracking().SingleOrDefault(i => i.Current == true).EndYear, 5, 1),
        //    };
        //    _context.CommitieMembership.Add(member);
        //    await _context.SaveChangesAsync();
        //    return Json("Success");
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("RemoveMember")]
        //public async Task<IActionResult> RemoveMember()
        //{
        //    int profId = int.Parse(Request.Form["ProfID"]);
        //    int commId = int.Parse(Request.Form["CommID"]);
        //    var membership = _context.CommitieMembership.Where(i => i.CommitteeID == commId && i.ProfessorID == profId && i.FinishedWork != true).SingleOrDefault();
        //    membership.EndDate = DateTime.Now;
        //    membership.FinishedWork = true;
        //    if(membership.Chair == true)
        //    {
        //        membership.Chair = false;
        //        membership.Committee.ProfessorID = null;
        //    }
        //    //_context.CommitieMembership.Remove(membership);
        //    await _context.SaveChangesAsync();
        //    return Json("Success");
        //}

    }
}