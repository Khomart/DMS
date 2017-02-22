using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Identity;

namespace ContosoUniversity.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context,
            UserManager<IdentityUser<int>> userManager,
            SignInManager<IdentityUser<int>> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public UserManager<IdentityUser<int>> _userManager { get; private set; }

        public SignInManager<IdentityUser<int>> _signInManager { get; private set; }



        public IActionResult Index()
        {
            if (User.IsInRole("Student")) return RedirectToAction("StudentIndex", "Student");
            else if (User.IsInRole("Professor")) return RedirectToAction("ProfessorIndex", "Professor");
            else if (User.IsInRole("Admin")) return RedirectToAction("AdminIndex", "Admin");
            //else if (User.IsInRole("Admin")) return RedirectToAction("AdminIndex", "Admin");
            else return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }


        //public async Task<IActionResult> StudentIndex()
        //{
        //    //int id = 6;
        //    ViewData["Message"] = "Default for student.";
        //    //var user = User.Identity;
        //    //Student tempuser = _context.Students.FirstOrDefault(x => x.Email == user);
        //    //int? id = tempuser.Id;
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    //if (id == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    var student = await _context.Students
        //        .Include(s => s.Enrollments)
        //            .ThenInclude(e => e.Course)
        //        .AsNoTracking()
        //        .SingleOrDefaultAsync(m => m.Id == user.Id);

        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(student);
        //}
        //// GET: Students/Edit/5
        //public async Task<IActionResult> StudentCourseEdit(int? id)
        //{
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (id != user.Id) return RedirectToAction("AccessDenied", "Account");


        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var student = await _context.Students.Include(i => i.Enrollments)
        //        .ThenInclude(i => i.Course).SingleOrDefaultAsync(m => m.Id == id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }
        //    //var listofenr = new List<Enrollment>();
        //    //foreach (var enrollment in student.Enrollments)
        //    //{
        //    //    listofenr.Add(enrollment);
        //    //}
        //    PopulateEnrolledCourseData(student);

        //    return View(student);

        //}

        //// POST: Students/Edit/5
        ////custom method to add/remove MYcourses (student)
        //[HttpPost, ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> StudentCourseEditPost(int? id, string[] selectedCourses)
        //{
        //    if (id == null)//method called with form which accepts hidden ID
        //    {
        //        return NotFound();
        //    }
        //    //get provided user (from id)
        //    Student student = await _context.Students
        //        .Include(i => i.Enrollments)
        //        .ThenInclude(i => i.Course)
        //        .SingleOrDefaultAsync(m => m.Id == id);

        //    //comparing user and person who calls this action
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (student.Id != user.Id) return RedirectToAction("AccessDenied", "Account");
        //    if (selectedCourses != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            //creat empty list for entities
        //            student.Enrollments = new List<Enrollment>();
        //            var selectedCoursesHS = new HashSet<string>(selectedCourses);
        //            var studentEnrollments = new HashSet<int>
        //(student.Enrollments.Select(c => c.Course.CourseID));
        //            //loop to add/remove courses
        //            foreach (var course in _context.Courses)
        //            {
        //                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
        //                {
        //                    if (!studentEnrollments.Contains(course.CourseID))
        //                    {
        //                        student.Enrollments.Add(new Enrollment { SmID = student.Id, CourseID = course.CourseID });
        //                    }
        //                }
        //                else
        //                {

        //                    if (studentEnrollments.Contains(course.CourseID))
        //                    {
        //                        Enrollment courseToRemove = student.Enrollments.SingleOrDefault(i => i.CourseID == course.CourseID);
        //                        _context.Remove(courseToRemove);
        //                    }
        //                }
        //            }
        //            var result = await TryUpdateModelAsync<Student>(student, "", di => di.Enrollments);
        //            try
        //            {
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateException /* ex */)
        //            {
        //                //Log the error (uncomment ex variable name and write a log.)
        //                ModelState.AddModelError("", "Unable to save changes. " +
        //                    "Try again, and if the problem persists, " +
        //                    "see your system administrator.");
        //            }
        //        }
        //    }
        //    return RedirectToAction("StudentIndex");
        //}


        //    private void PopulateEnrolledCourseData(Student student)
        //    {
        //        var allCourses = _context.Courses;
        //        if (student.Enrollments == null)
        //            student.Enrollments = new List<Enrollment>();
        //        var studentCourses = new HashSet<int>(student.Enrollments.Select(c => c.Course.CourseID));

        //        var viewModel = new List<StudentCourseData>();
        //        foreach (var course in allCourses)
        //        {
        //            viewModel.Add(new StudentCourseData
        //            {
        //                CourseID = course.CourseID,
        //                Title = course.Title,
        //                Assigned = studentCourses.Contains(course.CourseID),

        //            });
        //        }
        //        ViewData["Courses"] = viewModel;
        //    }
        //}

        
    }
}
