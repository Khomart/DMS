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

namespace ContosoUniversity.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    [Authorize(Roles = "Admin, Professor")]
    public class CommitteesController : Controller
    {
        private readonly SchoolContext _context;

        public CommitteesController(SchoolContext context, IHostingEnvironment environment, UserManager<IdentityUser<int>> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;

        }
        private IHostingEnvironment _environment;
        public UserManager<IdentityUser<int>> _userManager { get; private set; }

        // GET: Committees
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Index()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            string areaName = ControllerContext.RouteData.Values["area"].ToString();
            string[] acab = Routes.ReturnRoutes(ControllerContext);
            IndexView view = new IndexView();
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user.Id != 1)
            {
                view = new IndexView()
                {
                    Commitees = _context.Committees.AsNoTracking().Include(c => c.Chair).Include(c => c.Department).Include(c => c.Faculty).Where(m => m.Archived == false).Include(c => c.Chair).Include(c => c.Department),
                    MembershipIn = _context.CommitieMembership.AsNoTracking().Where(i => i.ProfessorID == user.Id && i.FinishedWork == false).ToList()
                };
            }
            else
            {
                view = new IndexView()
                {
                    Commitees = _context.Committees.AsNoTracking().Include(c => c.Chair).Include(c => c.Department).Include(c => c.Faculty).Where(m => m.Archived == false).Include(c => c.Chair).Include(c => c.Department),
                    MembershipIn = null
                };
            }
            //var schoolContext = _context.Committees.Where(m => m.Archived == false).Include(c => c.Chair).Include(c => c.Department);

            return View(view);
        }

        // GET: Committees/Details/5
        [Authorize(Roles = "Admin, Professor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var committee = await _context.Committees.AsNoTracking().Include(i => i.Chair).SingleOrDefaultAsync(m => m.CommitteeID == id && m.Archived == false);
            if (committee == null)
            {
                return NotFound();
            }

            return View(committee);
        }

        [Authorize(Roles = "Admin, Professor")]
        [ActionName("ViewCommittee")]
        public async Task<IActionResult> ViewCommittee(int? id, int? year, int? membership)
        {
            if (id == null)
            {
                return NotFound();
            }
            var committee = await _context.Committees
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
                .SingleOrDefaultAsync(i => i.CommitteeID == id && i.Archived == false);
            if (committee == null) return RedirectToAction("AccessDenied", "Account");
            var privelegy = await CheckPrivelegy((int)id);
            if ((bool)privelegy[1] == false)
            {
                return NotFound();
            }
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
                meetings = committee.Meetings.Where(i => i.OpenDate.Year == DateTime.Today.Year && i.Archived == false).ToList();
            }
            else if (year != 0)
            {
                if (membership == 2)
                    members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == year).OrderByDescending(i => i.DateOfEnrollment).ToList();
                if (membership == 1)
                    members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == year && i.FinishedWork == true).OrderByDescending(i => i.DateOfEnrollment).ToList();
                if (membership == 0 || membership == null)
                    members = committee.CommitieMembers.Where(i => i.DateOfEnrollment.Year == year && i.FinishedWork == false).OrderByDescending(i => i.DateOfEnrollment).ToList();
                meetings = committee.Meetings.Where(i => i.OpenDate.Year == year && i.Archived == false).ToList();
            }
            else
            {
                if (membership == 2)
                    members = committee.CommitieMembers.OrderByDescending(i => i.DateOfEnrollment).ToList();
                else if (membership == 1)
                    members = committee.CommitieMembers.Where(i => i.FinishedWork == true).OrderByDescending(i => i.DateOfEnrollment).ToList();
                else if (membership == 0 || membership == null)
                    members = committee.CommitieMembers.Where(i => i.FinishedWork == false).OrderByDescending(i => i.DateOfEnrollment).ToList();
                meetings = committee.Meetings.Where(i => i.Archived == false).ToList();
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
            ViewData["UserID"] = (int)privelegy[0];
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

        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("RemoveFromCommittee")]
        public async Task<IActionResult> RemoveFromCommittee(int? ProfID, int? CommID)
        {
            if (ProfID == null || CommID == null) return NotFound();
            var privelegy = await CheckPrivelegy((int)CommID);
            if ((bool)privelegy[2] == false)
            {
                return Json("NotSufficientRights");
            }

            var membership = _context.CommitieMembership
                .Where(i => i.CommitteeID == CommID && i.ProfessorID == ProfID && i.FinishedWork != true)
                .SingleOrDefault();
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

        [Authorize(Roles = "Admin, Professor")]
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
                var profs = await _context.Professors.Include(i => i.Department).ThenInclude(i => i.Faculty).Include(i => i.Commities).Where(i => i.Archived == false).AsNoTracking().ToListAsync();
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
                ViewData["comTitle"] = _context.Committees.Single(i => i.CommitteeID == (int)id).Title;
                ViewData["comID"] = (int)id;
                return View(InviteList);
            }
        }

        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("Invite")]
        public async Task<IActionResult> Invite(int ProfID, int CommID)
        {
            var privelegy = await CheckPrivelegy((int)CommID);
            if ((bool)privelegy[2] == false)
            {
                return Json("NotSufficientRights");
            }
            //int profId = int.Parse(Request.Form["ProfID"]);
            //int commId = int.Parse(Request.Form["CommID"]);
            CommitieMembership member = new CommitieMembership()
            {
                Chair = false,
                ProfessorID = ProfID,
                CommitteeID = CommID,
                FinishedWork = false,
                DateOfEnrollment = DateTime.Today.Date,
                EstimatedEndDate = new DateTime(_context.Semesters.AsNoTracking().SingleOrDefault(i => i.Current == true).EndYear, 5, 1),
            };
            _context.CommitieMembership.Add(member);
            await _context.SaveChangesAsync();
            return Json("Success");
        }

        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("RemoveMember")]
        public async Task<IActionResult> RemoveMember(int ProfID, int CommID)
        {
            var privelegy = await CheckPrivelegy((int)CommID);
            if ((bool)privelegy[2] == false)
            {
                return Json("NotSufficientRights");
            }
            //int profId = int.Parse(Request.Form["ProfID"]);
            //int commId = int.Parse(Request.Form["CommID"]);
            var membership = _context.CommitieMembership.Where(i => i.CommitteeID == CommID && i.ProfessorID == ProfID && i.FinishedWork != true).SingleOrDefault();
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

        [Authorize(Roles = "Admin, Professor")]
        [HttpGet, ActionName("SetMeeting")]
        public async Task<IActionResult> SetMeeting(int? id)
        {
            var privelegy = await CheckPrivelegy((int)id);
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }
            Committee committee = await _context.Committees.AsNoTracking().SingleAsync(i => i.CommitteeID == id);
            ViewData["CommitteeID"] = committee.CommitteeID;
            ViewData["Title"] = committee.Title;
            ViewData["comTitle"] = _context.Committees.Single(i => i.CommitteeID == (int)id).Title;
            return View();

        }
        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("SetMeeting")]
        public async Task<IActionResult> SetMeeting(Meetings model, string CommitteeID, DateTime[] Suggestions)
        {
            var privelegy = await CheckPrivelegy(int.Parse(CommitteeID));
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            model.Archived = false;
            if (ModelState.IsValid && Suggestions.Count() > 0)
            {
                model.Suggestions = new List<DatesSuggestion>();
                DatesSuggestion date;
                for (int i = 0; i < Suggestions.Count(); i++)
                {
                    date = new DatesSuggestion()
                    {
                        Value = Suggestions[i],
                    };
                    model.Suggestions.Add(date);
                }
                model.CommitteeID = int.Parse(CommitteeID);
                model.FinalDate = false;
                model.OpenDate = DateTime.Today;
                _context.Meetings.Add(model);
                await _context.SaveChangesAsync();
                Committee committee = await _context.Committees.SingleAsync(i => i.CommitteeID == model.CommitteeID);
                return RedirectToAction("ViewCommittee", new { id = committee.CommitteeID });
            }

            ViewData["CommitteeID"] = model.CommitteeID;
            ViewData["Title"] = model.Title;
            ViewData["comTitle"] = _context.Committees.Single(i => i.CommitteeID == model.CommitteeID).Title;
            return View(model);
        }
        [Authorize(Roles = "Admin, Professor")]
        [HttpGet, ActionName("EditMeeting")]
        public async Task<IActionResult> EditMeeting(int? comID, int? mtnID)
        {
            if (comID == null || mtnID == null)
            {
                return NotFound();
            }
            var privelegy = await CheckPrivelegy((int)comID);
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            Meetings meeting = await _context.Meetings.Include(i => i.Committee).SingleAsync(i => i.CommitteeID == comID && i.MeetingID == mtnID);

            ViewData["CommitteeID"] = meeting.CommitteeID;
            ViewData["comTitle"] = _context.Committees.Single(i => i.CommitteeID == meeting.CommitteeID).Title; return View(meeting);

        }
        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("DeleteMeeting")]
        public async Task<IActionResult> DeleteMeeting(int? ID, int? comID)
        {
            if (ID == null || comID == null)
            {
                return NotFound();
            }
            var privelegy = await CheckPrivelegy((int)comID);
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            Meetings meeting = await _context.Meetings.SingleAsync(i => i.MeetingID == ID);
            meeting.Archived = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewCommittee", new { id = meeting.CommitteeID });
        }
        [Authorize(Roles = "Admin, Professor")]
        [ActionName("ViewMeeting")]
        public async Task<IActionResult> ViewMeeting(int? comID, int? mtnID, int? page)
        {
            if (comID == null || mtnID == null)
            {
                return NotFound();
            }
            var privelegy = await CheckPrivelegy((int)comID);
            if ((bool)privelegy[1] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            MeetingView view = new MeetingView();//sdelat' chto-to s comment owner (might be admin, which cant be casted in prof)
            view.Meeting = await _context
                .Meetings
                .Include(i => i.Files)
                .Include(i => i.Comments)
                //.Include(i => i.Files)
                .Include(i => i.Committee)
                .SingleAsync(i => i.CommitteeID == comID && i.MeetingID == mtnID);
            if(view.Meeting.Archived || view.Meeting.Committee.Archived) return RedirectToAction("AccessDenied", "Account");
            //view.PublicComments =  view.Meeting.Comments.Where(i => i.Private == false).ToList().OrderByDescending(d => d.DateStamp);
            view.PublicComments = PaginatedList<MeetingComment>.Create(view.Meeting.Comments.Where(i => i.Private == false).ToList().OrderByDescending(d => d.DateStamp), page ?? 1, 10);
            view.PrivateComments = view.Meeting.Comments.Where(i => i.Private == true && i.ProfessorID == (int)privelegy[0]).ToList().OrderByDescending(d => d.DateStamp);

            var PublicFiles = view.Meeting.Files.Where(i => i.OwnerID == (int)privelegy[0] && i.Owned == Ownership.meetingPub).ToList().OrderByDescending(d => d.Added);
            List<FilesAssosiation> publicFiles = new List<FilesAssosiation>();

            foreach (var file in PublicFiles)
            {
                FilesAssosiation newFile = new FilesAssosiation();
                newFile.File = file;
                if (file.OwnerID != 1) newFile.Author = _context.Professors.Single(i => i.Id == file.OwnerID).FullName;
                else newFile.Author = "Administrator";
                publicFiles.Add(newFile);
            }
            view.PublicFiles = publicFiles;

            var PrivateFiles = view.Meeting.Files.Where(i => i.OwnerID == (int)privelegy[0] && i.Owned == Ownership.meetingPriv).ToList().OrderByDescending(d => d.Added);
            List<FilesAssosiation> privateFiles = new List<FilesAssosiation>();
            foreach (var file in PrivateFiles)
            {
                FilesAssosiation newFile = new FilesAssosiation();
                newFile.File = file;
                if (file.OwnerID != 1) newFile.Author = _context.Professors.Single(i => i.Id == file.OwnerID).FullName;
                else newFile.Author = "Administrator";
                privateFiles.Add(newFile);
            }
            view.PrivateFiles = privateFiles;
            //if (User.IsInRole("Professor"))
            //{
            //    foreach (var file in PrivateFiles)
            //        privateFiles.Add(new FilesAssosiation()
            //        {
            //            File = file,
            //            Author = _context.Professors.Single(i => i.Id == file.OwnerID).FullName,
            //        });
            //    view.PrivateFiles = privateFiles;
            //}
            //else if (User.IsInRole("Admin"))
            //{
            //    foreach (var file in PrivateFiles)
            //        privateFiles.Add(new FilesAssosiation()
            //        {
            //            File = file,
            //            Author = "Administrator",
            //        });
            //    view.PrivateFiles = privateFiles;
            //}

            ViewData["CommitteeID"] = view.Meeting.CommitteeID;
            ViewData["ProfessorID"] = (int)privelegy[0];
            ViewData["Moderator"] = (bool)privelegy[2];
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

        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("EditMeeting")]
        public async Task<IActionResult> EditMeeting(Meetings model, string CommitteeID)
        {
            var privelegy = await CheckPrivelegy(int.Parse(CommitteeID));
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var meetingToUpdate = _context.Meetings.Where(i => i.CommitteeID == model.CommitteeID && i.MeetingID == model.MeetingID).Single();
            if (ModelState.IsValid)
            {
                model.CommitteeID = int.Parse(CommitteeID);
                await TryUpdateModelAsync<Meetings>(
                meetingToUpdate,
                "",
                s => s.Location, s => s.Title/*, s => s.Date*/);
                //if (meetingToUpdate.Date == null) meetingToUpdate.Date = new DateTime(2000, 1, 1);
                await _context.SaveChangesAsync();
                Committee committee = await _context.Committees.SingleAsync(i => i.CommitteeID == model.CommitteeID);
                return RedirectToAction("ViewCommittee", new { id = committee.CommitteeID });
            }

            ViewData["CommitteeID"] = model.CommitteeID;
            ViewData["comTitle"] = _context.Committees.Single(i => i.CommitteeID == model.CommitteeID).Title;
            return View(model);
        }
        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("CommentAdd")]
        public async Task<IActionResult> CommentAdd(MeetingComment model/*, ICollection<IFormFile> files*/)
        {

            if (ModelState.IsValid)
            {
                Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);
                var privelegy = await CheckPrivelegy(meeting.CommitteeID);
                if ((bool)privelegy[1] == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                //if (meeting.Files == null) model.Files = new List<FileBase>();
                if ((int)privelegy[0] == 1) { model.ProfessorName = "DMS_Admin"; model.ProfessorID = null; }
                else model.ProfessorName = _context.Professors.SingleOrDefault(i => i.Id == (int)privelegy[0]).FullName;
                model.DateStamp = DateTime.Now;
                _context.MeetComments.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });
            }
            return View(model);
        }

        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int? ID)
        {
            if (ID == null) return NotFound();
            var comment = await _context.MeetComments.SingleOrDefaultAsync(i => i.CommentID == ID);
            if (comment != null)
            {
                var privelegy = await CheckPrivelegy(comment.CommitteeID);
                if ((bool)privelegy[2])
                {
                    _context.Remove(comment);
                    await _context.SaveChangesAsync();
                    return Json("success");
                }
                else if((int)privelegy[0] == comment.ProfessorID){
                    double time = (DateTime.Now - comment.DateStamp).TotalMinutes;
                    if(time < 10)
                    {
                        _context.Remove(comment);
                        await _context.SaveChangesAsync();
                        return Json("success");
                    }
                }
            }
            return Json("error");
        }

        [Authorize(Roles = "Admin, Professor")]
        [ActionName("EditComment")]
        public async Task<IActionResult> EditComment(int? ID)
        {
            if (ID == null) return NotFound();
            var comment = await _context.MeetComments.SingleOrDefaultAsync(i => i.CommentID == ID);
            if (comment != null)
            {
                var privelegy = await CheckPrivelegy(comment.CommitteeID);
                if ((bool)privelegy[2])
                {
                    return Json(comment.Comment);
                    //_context.Remove(comment);
                    //await _context.SaveChangesAsync();
                    //return Json("success");
                }
                else if ((int)privelegy[0] == comment.ProfessorID)
                {
                    double time = (DateTime.Now - comment.DateStamp).TotalMinutes;
                    if (time < 10)
                    {
                        return Json(comment.Comment);
                    }
                }
            }
            return Json("error getting comment");
        }

        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("EditComment")]
        public async Task<IActionResult> CommentEditSubmit(int? ID, string comment)
        {
            if(ID != null && comment != null)
            {
                var commentEntity = await _context.MeetComments.SingleOrDefaultAsync(i => i.CommentID == ID);
                if (commentEntity != null)
                {
                    var privelegy = await CheckPrivelegy(commentEntity.CommitteeID);
                    if ((bool)privelegy[2])
                    {
                        commentEntity.Comment = comment;
                        await _context.SaveChangesAsync();
                        //_context.Remove(comment);
                        //await _context.SaveChangesAsync();
                        //return Json("success");
                    }
                    else if ((int)privelegy[0] == commentEntity.ProfessorID)
                    {
                        double time = (DateTime.Now - commentEntity.DateStamp).TotalMinutes;
                        if (time < 10)
                        {
                            commentEntity.Comment = comment;
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("ViewMeeting", new { comID = commentEntity.CommitteeID, mtnID = commentEntity.MeetingID });
                }
            }
            return RedirectToAction("AccessDenied", "Account");
        }
        [Authorize(Roles = "Professor, Admin")]
        [HttpPost, ActionName("FileAdd")]
        public async Task<IActionResult> FileAdd(MeetingComment model, ICollection<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                Meetings meeting = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID);
                var privelegy = await CheckPrivelegy(meeting.CommitteeID);
                if ((bool)privelegy[1] == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
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
                                OwnerID = (int)model.ProfessorID,
                                Added = DateTime.Now,
                                ViewTitle = name,
                            };
                            if (User.IsInRole("Admin")) fileToSave.OwnerID = 1;
                            meeting.Files.Add(fileToSave);
                        }
                        else
                        {
                            FileBase fileToSave = new FileBase()
                            {
                                Location = "/uploads/Committees/" + fileName,
                                Owned = Ownership.meetingPub,
                                OwnerID = (int)model.ProfessorID,
                                Added = DateTime.Now,
                                ViewTitle = name,
                            };
                            if (User.IsInRole("Admin")) fileToSave.OwnerID = 1;
                            meeting.Files.Add(fileToSave);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewMeeting", new { comID = meeting.CommitteeID, mtnID = meeting.MeetingID });
            }
            return RedirectToAction("ProfessorIndex");
        }

        [Authorize(Roles = "Admin, Professor")]
        [ActionName("Suggestions")]
        public async Task<IActionResult> Suggestions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChooseMeetingDate set = new ChooseMeetingDate();
            List<DatesSuggestion> suggestions = await _context.DatesSuggestion.Include(i => i.Checkers).Where(i => i.MeetingID == id).ToListAsync();
            var privelegy = await CheckPrivelegy(suggestions[0].CommitteeID);
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            set.Members = _context.Meetings.Include(c => c.Committee)
                .ThenInclude(c => c.CommitieMembers)
                .ThenInclude(c => c.Professor)
                .Single(i => i.MeetingID == id)
                .Committee.CommitieMembers
                .Where(i => i.FinishedWork == false && i.Professor != i.Committee.Chair)
                .ToList();
            set.Dates = _context.DatesSuggestion.Where(i => i.MeetingID == id).ToList();
            ViewBag.Dates = new SelectList(set.Dates, "Value", "Value");
            ViewData["MeetingID"] = id;
            ViewData["CommitteeID"] = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == (int)id).Committee.CommitteeID;
            ViewData["comTitle"] = _context.Committees.Single(i => i.CommitteeID == (int)ViewData["CommitteeID"]).Title;
            return View(set);
        }
        [Authorize(Roles = "Admin, Professor")]
        [HttpPost, ActionName("SetMeetingDate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMeetingDate(int MeetingID, string Date)
        {
            DateTime date = Convert.ToDateTime(Request.Form["Date"]);
            Meetings meeting = _context.Meetings.SingleOrDefault(i => i.MeetingID == MeetingID);
            var privelegy = await CheckPrivelegy(meeting.CommitteeID);
            if ((bool)privelegy[2] == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            meeting.Date = date;
            meeting.FinalDate = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewCommittee", new { id = meeting.CommitteeID });
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
        [HttpPost, ActionName("SuggestDate")]
        public async Task<IActionResult> SuggestDate(ChoseDates model)
        {
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            Professor professor = _context.Professors.Single(i => i.Id == user.Id);
            if (ModelState.IsValid)
            {
                DatesSuggestion suggestion;
                foreach (var date in model.Dates)
                    if (date.choice == true)
                    {
                        suggestion = _context.DatesSuggestion.Include(i => i.Checkers).Single(i => i.SuggestionID == date.date.SuggestionID);
                        suggestion.Checkers.Add(professor);
                    }
                await _context.SaveChangesAsync();
                Committee committee = _context.Meetings.Include(i => i.Committee).Single(i => i.MeetingID == model.MeetingID).Committee;
                return RedirectToAction("ViewCommittee", new { id = committee.CommitteeID });
            }
            return View(model);
        }
        private async Task<object[]> CheckPrivelegy(int comID)
        {
            //returns sequence: userID, access rights, change rights.
            //data[0] = id, data[1] = is member, data[2] = is moderator
            object[] data = new object[3];
            bool check = false;
            IdentityUser<int> user = await _userManager.FindByNameAsync(User.Identity.Name);
            data[0] = user.Id;
            Committee committee = await _context.Committees.SingleOrDefaultAsync(i => i.CommitteeID == comID);
            foreach (CommitieMembership member in _context.CommitieMembership.Where(i => i.CommitteeID == comID))
            {
                if (member.ProfessorID == user.Id && member.FinishedWork == false)
                {
                    check = true;
                    break;
                }
            }
            if (User.IsInRole("Admin")) check = true;
            if (check == true) data[1] = true;
            else data[1] = false;

            Committee committie = await _context.Committees.SingleAsync(i => i.CommitteeID == comID);
            if (user.Id == 1 || committie.ProfessorID == user.Id) data[2] = true;
            else data[2] = false;

            return data;
        }

    }
}