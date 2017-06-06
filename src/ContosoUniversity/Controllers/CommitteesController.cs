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

namespace ContosoUniversity.Controllers
{
    [Authorize(Roles = "Admin, Professor")]
    public class CommitteesController : Controller
    {
        private readonly SchoolContext _context;

        public CommitteesController(SchoolContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }
        private IHostingEnvironment _environment;

        // GET: Committees
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Committees.AsNoTracking().Where( m => m.Archived == false).Include(c => c.Chair).Include(c => c.Department);
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

            var committee = await _context.Committees.AsNoTracking().SingleOrDefaultAsync(m => m.CommitteeID == id && m.Archived == false);
            if (committee == null)
            {
                return NotFound();
            }

            return View(committee);
        }

        [Authorize(Roles = "Admin")]
        [ActionName("CreateCommitie")]
        public async Task<IActionResult> CreateCommitie()
        {
            //ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "FullName");
            //ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
            ViewData["ProfessorID"] = PopulateDropdown.Populate(_context, "professor");
            ViewData["Professor"] = await _context.Professors.AsNoTracking().ToListAsync();
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department",null, "Faculty.Name");
            ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");
            //Committee committee = new Committee();
            //committee.SemesterID = _context.Semesters.Where(i => i.current == true).SingleOrDefault().ID;
            return View(/*committee*/);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("CreateCommitie")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCommitie([Bind("DepartmentID,ProfessorID,FacultyID,Commentary,Title,Level")] Committee model)
        {
            if (ModelState.IsValid)
            {
                if (model.Level == Level.Department) model.FacultyID = null;
                else if (model.Level == Level.Faculty) model.DepartmentID = null;
                else if (model.Level == Level.University) { model.FacultyID = null; model.DepartmentID = null; }
                if (model.ProfessorID != null)
                {
                    DateTime end = new DateTime();
                    if (DateTime.Today < new DateTime(DateTime.Today.Year, 6, 1)) end = new DateTime(DateTime.Today.Year, 6, 1);
                    else end = new DateTime(DateTime.Today.Year + 1, 6, 1);
                    CommitieMembership member = new CommitieMembership();
                    if (model.ProfessorID != null)
                        member = new CommitieMembership()
                        {
                            Chair = true,
                            ProfessorID = (int)model.ProfessorID,
                            DateOfEnrollment = DateTime.Now,
                            EstimatedEndDate = end,
                        };
                    else
                        member = new CommitieMembership()
                        {
                            Chair = true,
                            DateOfEnrollment = DateTime.Now,
                            EstimatedEndDate = end,
                        };
                    model.CommitieMembers = new List<CommitieMembership>()
                    {
                        member,
                    };
                }
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ProfessorID"] = PopulateDropdown.Populate(_context, "professor");
            ViewData["Professor"] = await _context.Professors.AsNoTracking().ToListAsync();
            ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", null, "Faculty.Name");
            ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [ActionName("ManageCommitie")]
        public async Task<IActionResult> ManageCommitie(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Committee committee = await _context.Committees
                .AsNoTracking()
                .Include(i => i.CommitieMembers)
                .ThenInclude(i => i.Professor)
                .ThenInclude(i => i.OfficeAssignment)
                .SingleOrDefaultAsync(i => i.CommitteeID == id);
            ViewData["ProfessorID"] = PopulateDropdown.Populate(_context, "professor", committee.ProfessorID);
            //ViewData["ProfessorID"] = new SelectList(_context.Professors, "Id", "FullName", committee.ProfessorID);
            if (committee.Level == Level.Department)
            {
                ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", committee.DepartmentID, "Faculty.Name");
                //ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", committee.DepartmentID, "Faculty.Name");
                ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");

            }
            else if (committee.Level == Level.Faculty)
            {
                ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", null , "Faculty.Name");
                //ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
                ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name", committee.FacultyID);
            }
            else if (committee.Level == Level.University)
            {
                ViewData["DepartmentID"] = PopulateDropdown.Populate(_context, "department", null, "Faculty.Name");
                //ViewData["DepartmentID"] = new SelectList(_context.Departments.Include(i => i.Faculty), "DepartmentID", "Name", null, "Faculty.Name");
                ViewData["FacultyID"] = new SelectList(_context.Facultys.Include(i => i.Departments), "FacultyID", "Name");
            }
            ViewData["Professor"] = await _context.Professors.AsNoTracking().ToListAsync();
            return View(committee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("UpdateCommittee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCommittee(Committee model)
        {
            if (model.Level == Level.Department) model.FacultyID = null;
            else if (model.Level == Level.Faculty) model.DepartmentID = null;
            else if (model.Level == Level.University) { model.FacultyID = null; model.DepartmentID = null; }

            var committeeToUpdate = await _context.Committees.Include(i => i.CommitieMembers).SingleOrDefaultAsync(s => s.CommitteeID == model.CommitteeID);
            if (model.ProfessorID == null && committeeToUpdate.ProfessorID == null){}
            else if (model.ProfessorID == null && committeeToUpdate.ProfessorID != null)
            {
                committeeToUpdate.ProfessorID = null;
                committeeToUpdate.CommitieMembers.SingleOrDefault(i => i.Chair == true).Chair = false;
            }
            else if (model.ProfessorID != null && committeeToUpdate.ProfessorID != model.ProfessorID)
            {
                bool found = false;
                foreach (var member in committeeToUpdate.CommitieMembers)
                {
                    if (member.ProfessorID == model.ProfessorID && member.Chair == false)
                    {
                        member.Chair = true;
                        found = true;
                    }
                    else if (member.ProfessorID != model.ProfessorID && member.Chair == true)
                    {
                        member.Chair = false;
                    }
                    else if (member.ProfessorID == model.ProfessorID && member.Chair == true)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    DateTime end = new DateTime();
                    if (DateTime.Today < new DateTime(DateTime.Today.Year, 6, 1)) end = new DateTime(DateTime.Today.Year, 6, 1);
                    else end = new DateTime(DateTime.Today.Year + 1, 6, 1);
                    CommitieMembership member = new CommitieMembership()
                    {
                        Chair = true,
                        ProfessorID = (int)model.ProfessorID,
                        CommitteeID = model.CommitteeID,
                        DateOfEnrollment = DateTime.Now,
                        EstimatedEndDate = end,
                    };
                    _context.CommitieMembership.Add(member);
                }

            }
            if (await TryUpdateModelAsync<Committee>(
                committeeToUpdate,
                "",
                s => s.CommitteeID, s => s.ProfessorID, s => s.FacultyID, s => s.Title, s => s.Level, s => s.Commentary))
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else return RedirectToAction("ManageCommitie", new { id = model.CommitteeID });
        }

        [Authorize(Roles = "Admin")]
        [ActionName("ViewCommittee")]
        public async Task<IActionResult> ViewCommittee(int? id, int? year, int? membership)
        {
            if (id == null)
            {
                return NotFound();
            }
            Committee committee = await _context.Committees
                .Include(i => i.Chair)
                .Include(i => i.CommitieMembers)
                .ThenInclude(i => i.Professor)
                .ThenInclude(i => i.OfficeAssignment)
                .Include(i => i.CommitieMembers)
                .ThenInclude(i => i.Professor)
                .ThenInclude(i => i.Department)
                .Include(i => i.Meetings)
                .ThenInclude(i => i.Suggestions)
                .ThenInclude(i => i.Checkers)
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.CommitteeID == id);


                PopulateYearAndSemeter(year, null);

                ViewBag.Years = ViewData["years"];
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
                if (membership != null)
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
                semesters[(int)semester - 1].Selected = true;
            }
            ViewData["semesters"] = semesters;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("RemoveFromCommittee")]
        public async Task<IActionResult> RemoveFromCommittee(int? ProfID, int? CommID)
        {
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

        [Authorize(Roles = "Admin")]
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
                var profs = await _context.Professors.AsNoTracking().Include(i => i.Department).ThenInclude(i => i.Faculty).Include(i => i.Commities).Where(i => i.Archived == false).ToListAsync();
                for (int i = 0; i < profs.Count(); i++)
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
        [Authorize(Roles = "Admin")]
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
                DateOfEnrollment = DateTime.Now,
                EstimatedEndDate = new DateTime(_context.Semesters.AsNoTracking().SingleOrDefault(i => i.Current == true).EndYear, 5, 1),
            };
            _context.CommitieMembership.Add(member);
            await _context.SaveChangesAsync();
            return Json("Success");
        }
 
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("RemoveMember")]
        public async Task<IActionResult> RemoveMember()
        {
            int profId = int.Parse(Request.Form["ProfID"]);
            int commId = int.Parse(Request.Form["CommID"]);
            var membership = _context.CommitieMembership.Where(i => i.CommitteeID == commId && i.ProfessorID == profId && i.FinishedWork != true).SingleOrDefault();
            membership.EndDate = DateTime.Now;
            membership.FinishedWork = true;
            if (membership.Chair == true)
            {
                membership.Chair = false;
                membership.Committee.ProfessorID = null;
            }
            //_context.CommitieMembership.Remove(membership);
            await _context.SaveChangesAsync();
            return Json("Success");
        }


        //[Authorize(Roles = "Admin")]
        //[HttpGet, ActionName("SetMeeting")]
        //public async Task<IActionResult> SetMeeting(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    Committee committee = await _context.Committees.AsNoTracking().SingleAsync(i => i.CommitteeID == id);
        //    ViewData["CommitteeID"] = committee.CommitteeID;
        //    return View();

        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("SetMeeting")]
        //public async Task<IActionResult> SetMeeting(Meetings model, string CommitteeID)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.Suggestions = new List<DatesSuggestion>();
        //        DatesSuggestion date;
        //        DateTime datetime;
        //        for (int i = 0; i < 5; i++)
        //        {
        //            var suggestion = Request.Form["Suggestions[" + i + "]"];
        //            if (suggestion != "")
        //            {
        //                datetime = Convert.ToDateTime(Request.Form["Suggestions[" + i + "]"]);
        //                date = new DatesSuggestion()
        //                {
        //                    Value = datetime,
        //                };
        //                model.Suggestions.Add(date);
        //            }
        //        }
        //        model.CommitteeID = int.Parse(CommitteeID);
        //        model.FinalDate = false;
        //        model.OpenDate = DateTime.Today;
        //        _context.Meetings.Add(model);
        //        await _context.SaveChangesAsync();
        //        Committee committee = await _context.Committees.SingleAsync(i => i.CommitteeID == model.CommitteeID);
        //        return RedirectToAction("ViewCommittee", new { id = committee.CommitteeID });
        //    }
        //    return View(model);
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpGet, ActionName("EditMeeting")]
        //public async Task<IActionResult> EditMeeting(int? comID, int? mtnID)
        //{
        //    if (comID == null || mtnID == null)
        //    {
        //        return NotFound();
        //    }
        //    Meetings meeting = await _context.Meetings.Include(i => i.Committee).SingleAsync(i => i.CommitteeID == comID && i.MeetingID == mtnID);
        //    ViewData["CommitteeID"] = meeting.CommitteeID;
        //    return View(meeting);

        //}
        //[Authorize(Roles = "Admin")]
        //[ActionName("ViewMeeting")]
        //public async Task<IActionResult> ViewMeeting(int? comID, int? mtnID, int? page)
        //{
        //    if (comID == null || mtnID == null)
        //    {
        //        return NotFound();
        //    }
        //    MeetingView view = new MeetingView();
        //    view.Meeting = _context.Meetings.Include(i => i.Files).Include(i => i.Comments)/*.ThenInclude(i => i.Files)*/.Include(i => i.Committee).Single(i => i.CommitteeID == comID && i.MeetingID == mtnID);
        //    //view.PublicComments = view.Meeting.Comments.Where(i => i.Private == false).ToList();
        //    view.PublicComments = PaginatedList<MeetingComment>.Create(view.Meeting.Comments.Where(i => i.Private == false).ToList().OrderByDescending(d => d.DateStamp), page ?? 1, 10);

        //    view.PrivateComments = view.Meeting.Comments.Where(i => i.Private == true && i.ProfessorID == 1).ToList();

        //    var PublicFiles = view.Meeting.Files.Where(i => i.OwnerID == mtnID && i.Owned == Ownership.meetingPub).ToList();
        //    List<FilesAssosiation> publicFiles = new List<FilesAssosiation>();
        //    foreach (var file in PublicFiles)
        //        publicFiles.Add(new FilesAssosiation()
        //        {
        //            File = file,
        //            Author = _context.Professors.Single(i => i.Id == file.OwnerID).FullName,
        //        });
        //    view.PublicFiles = publicFiles;

        //    var PrivateFiles = new List<FilesAssosiation>();
        //    view.PrivateFiles = PrivateFiles;

        //    ViewData["CommitteeID"] = view.Meeting.CommitteeID;
        //    ViewData["ProfessorID"] = 1;
        //    return View(view);
        //}
        //static string GetMd5Hash(MD5 md5Hash, string input)
        //{

        //    // Convert the input string to a byte array and compute the hash.
        //    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        //    // Create a new Stringbuilder to collect the bytes
        //    // and create a string.
        //    StringBuilder sBuilder = new StringBuilder();

        //    // Loop through each byte of the hashed data 
        //    // and format each one as a hexadecimal string.
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        sBuilder.Append(data[i].ToString("x2"));
        //    }

        //    // Return the hexadecimal string.
        //    return sBuilder.ToString();
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("EditMeeting")]
        //public async Task<IActionResult> EditMeeting(Meetings model, string CommitteeID)
        //{
        //    var meetingToUpdate = _context.Meetings.Where(i => i.CommitteeID == model.CommitteeID && i.MeetingID == model.MeetingID).Single();
        //    if (ModelState.IsValid)
        //    {
        //        model.CommitteeID = int.Parse(CommitteeID);
        //        await TryUpdateModelAsync<Meetings>(
        //        meetingToUpdate,
        //        "",
        //        s => s.Location, s => s.Title, s => s.Date);

        //        await _context.SaveChangesAsync();
        //        Committee committee = await _context.Committees.SingleAsync(i => i.CommitteeID == model.CommitteeID);
        //        return RedirectToAction("ViewCommittee", new { id = committee.CommitteeID });
        //    }
        //    return View(model);
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("CommentAdd")]
        //public async Task<IActionResult> CommentAdd(MeetingComment model/*, ICollection<IFormFile> files*/)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);


        //        //if (meeting.Files == null) model.Files = new List<FileBase>();
        //        model.ProfessorName = "DMS_Admin";
        //        model.DateStamp = DateTime.Now;
        //        _context.MeetComments.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });
        //    }
        //    return View(model);
        //}

        //[Authorize(Roles = "Professor")]
        //[HttpPost, ActionName("FileAdd")]
        //public async Task<IActionResult> FileAdd(MeetingComment model, ICollection<IFormFile> files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);

        //        if (files == null) return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });
        //        if (meeting.Files == null) meeting.Files = new List<FileBase>();

        //        var uploads = Path.Combine(_environment.WebRootPath, "uploads/Committees");
        //        foreach (var file in files)
        //        {
        //            if (file.Length > 0)
        //            {
        //                string name = file.FileName;
        //                MD5 hasher = MD5.Create();
        //                DateTime now = new DateTime();
        //                now = DateTime.Now;
        //                string fileName = GetMd5Hash(hasher, now.ToString()) + Path.GetExtension(file.FileName);

        //                using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
        //                {
        //                    await file.CopyToAsync(fileStream);
        //                }
        //                if (model.Private == true)
        //                {
        //                    FileBase fileToSave = new FileBase()
        //                    {
        //                        Location = "/uploads/Committees/" + fileName,
        //                        Owned = Ownership.meetingPriv,
        //                        OwnerID = model.ProfessorID,
        //                        Added = DateTime.Now,
        //                        ViewTitle = name,
        //                    };
        //                    meeting.Files.Add(fileToSave);
        //                }
        //                else
        //                {
        //                    FileBase fileToSave = new FileBase()
        //                    {
        //                        Location = "/uploads/Committees/" + fileName,
        //                        Owned = Ownership.meetingPub,
        //                        OwnerID = model.ProfessorID,
        //                        Added = DateTime.Now,
        //                        ViewTitle = name,
        //                    };
        //                    meeting.Files.Add(fileToSave);
        //                }
        //            }

        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });



        //        }
        //        return View(model);
        //    }
        //    return RedirectToAction("ProfessorIndex");
        //}

        //[Authorize(Roles = "Admin")]
        //[ActionName("Suggestions")]
        //public async Task<IActionResult> Suggestions(int? id)
        //{
        //    if(id == null)
        //    {
        //        return NotFound();
        //    }
        //    ChooseMeetingDate set = new ChooseMeetingDate();
        //    List<DatesSuggestion> suggestions = await _context.DatesSuggestion.Include(i => i.Checkers).Where(i => i.MeetingID == id).ToListAsync();
        //    set.Members = _context.Meetings.Include(c => c.Committee)
        //        .ThenInclude(c => c.CommitieMembers)
        //        .ThenInclude(c => c.Professor)
        //        .Single(i => i.MeetingID == id)
        //        .Committee.CommitieMembers
        //        .Where(i => i.FinishedWork == false && i.Professor != i.Committee.Chair)
        //        .ToList();
        //    set.Dates = _context.DatesSuggestion.Where(i => i.MeetingID == id).ToList();
        //    ViewBag.Dates = new SelectList(set.Dates, "Value", "Value");
        //    ViewData["MeetingID"] = id;
        //    ViewData["CommitteeID"] = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == (int)id).Committee.CommitteeID;
        //    return View(set);
        //}
        //[Authorize(Roles = "Admin")]
        //[HttpPost, ActionName("SetMeetingDate")]
        //public async Task<IActionResult> SetMeetingDate(int MeetingID, string Date)
        //{
        //    DateTime date = Convert.ToDateTime(Request.Form["Date"]);
        //    int meetingID = int.Parse(Request.Form["MeetingID"]);
        //    Meetings meeting = _context.Meetings.SingleOrDefault(i => i.MeetingID == meetingID);
        //    meeting.Date = date;
        //    meeting.FinalDate = true;
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("ViewCommittee", new { id = meeting.CommitteeID });
        //}

        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var committee = await _context.Committees.Include(p => p.Chair).SingleOrDefaultAsync(m => m.CommitteeID == id && m.Archived == false);
            if (committee == null)
            {
                return NotFound();
            }

            return View(committee);
        }

        // POST: Committees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmArchive(int id)
        {
            var committee = await _context.Committees.SingleOrDefaultAsync(m => m.CommitteeID == id);
            committee.Archived = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CommitteeExists(int id)
        {
            return _context.Committees.Any(e => e.CommitteeID == id);
        }
    }
}
