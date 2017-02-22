using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ContosoUniversity.Models.UniversityFunctionalityModels;

namespace ContosoUniversity.Models
{
    public static class DbInitializer
    {


        public static void Initialize(SchoolContext context)
        {

            // Look for any students.
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }

            var students = new Student[]
            {
                new Student {UserName = "Carson",
                    Email = "1@user.com", FirstMidName = "Carson",   LastName = "Alexander", Program = "BComp"},
                new Student {UserName = "Meredith",
                    Email = "2@user.com", FirstMidName = "Meredith", LastName = "Alonso", Program = "BEng"},
                new Student {UserName = "Arturo",
                    Email = "3@user.com", FirstMidName = "Arturo",   LastName = "Anand", Program = "BEng"},
                new Student {UserName = "Gytis",
                    Email = "4@user.com", FirstMidName = "Gytis",    LastName = "Barzdukas", Program = "BComp"},
                new Student {UserName = "Yan",
                    Email = "5@user.com", FirstMidName = "Yan",      LastName = "Li", Program = "BEng"},
                new Student {UserName = "Peggy",
                    Email = "6@user.com", FirstMidName = "Peggy",    LastName = "Justice", Program = "BComp"},
                new Student {UserName = "Laura",
                    Email = "7@user.com", FirstMidName = "Laura",    LastName = "Norman", Program = "BCSA"},
                new Student {UserName = "Nino",
                    Email = "8@user.com", FirstMidName = "Nino",     LastName = "Olivetto", Program = "BCSA"}
            };

            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            var professors = new Professor[]
            {
                new Professor {UserName = "Nino",
                    Email = "8@user.com", FirstMidName = "Kim",     LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11") },
                new Professor {UserName = "Nino",
                    Email = "8@user.com", FirstMidName = "Fadi",    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06") },
                new Professor {UserName = "Nino",
                    Email = "8@user.com", FirstMidName = "Roger",   LastName = "Harui",
                    HireDate = DateTime.Parse("1998-07-01") },
                new Professor {UserName = "Nino",
                    Email = "8@user.com", FirstMidName = "Candace", LastName = "Kapoor",
                    HireDate = DateTime.Parse("2001-01-15") },
                new Professor {UserName = "Nino",
                    Email = "8@user.com", FirstMidName = "Roger",   LastName = "Zheng",
                    HireDate = DateTime.Parse("2004-02-12") }
            };

            foreach (Professor i in professors)
            {
                context.Professors.Add(i);
            }
            context.SaveChanges();

            var faculties = new Faculty[]
            {
                new Faculty { Name = "Engineering and Computer Science",
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfessorID  = professors.Single( i => i.LastName == "Abercrombie").Id },
                new Faculty { Name = "John Molson School of Business",
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfessorID  = professors.Single( i => i.LastName == "Fakhouri").Id },
                new Faculty { Name = "Arts and Science",
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfessorID  = professors.Single( i => i.LastName == "Harui").Id },
                new Faculty { Name = "Fine Arts",
                    StartDate = DateTime.Parse("2007-09-01"),
                    ProfessorID  = professors.Single( i => i.LastName == "Kapoor").Id }
            };

            foreach (Faculty d in faculties)
            {
                context.Facultys.Add(d);
            }
            context.SaveChanges();

            var departments = new Department[]
            {
                new Department { Name = "English",
                    StartDate = DateTime.Parse("2007-09-01"),
                    FacultyID = faculties.Single(i => i.Name == ("Arts and Science")).FacultyID,
                    ProfessorID  = professors.Single( i => i.LastName == "Abercrombie").Id },
                new Department { Name = "Mathematics",
                    StartDate = DateTime.Parse("2007-09-01"),
                    FacultyID = faculties.Single(i => i.Name == ("Engineering and Computer Science")).FacultyID,
                    ProfessorID  = professors.Single( i => i.LastName == "Fakhouri").Id },
                new Department { Name = "Engineering",
                    StartDate = DateTime.Parse("2007-09-01"),
                    FacultyID = faculties.Single(i => i.Name == ("Engineering and Computer Science")).FacultyID,
                    ProfessorID  = professors.Single( i => i.LastName == "Harui").Id },
                new Department { Name = "Economics",
                    StartDate = DateTime.Parse("2007-09-01"),
                    FacultyID = faculties.Single(i => i.Name == ("John Molson School of Business")).FacultyID,
                    ProfessorID  = professors.Single( i => i.LastName == "Kapoor").Id }
            };

            foreach (Department d in departments)
            {
                context.Departments.Add(d);
            }
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course {CourseID = 1050, Title = "Chemistry",      Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Engineering").DepartmentID
                },
                new Course {CourseID = 4022, Title = "Microeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID
                },
                new Course {CourseID = 4041, Title = "Macroeconomics", Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "Economics").DepartmentID
                },
                new Course {CourseID = 1045, Title = "Calculus",       Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID
                },
                new Course {CourseID = 3141, Title = "Trigonometry",   Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "Mathematics").DepartmentID
                },
                new Course {CourseID = 2021, Title = "Composition",    Credits = 3,
                    DepartmentID = departments.Single( s => s.Name == "English").DepartmentID
                },
                new Course {CourseID = 2042, Title = "Literature",     Credits = 4,
                    DepartmentID = departments.Single( s => s.Name == "English").DepartmentID
                },
            };

            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            var officeAssignments = new OfficeAssignment[]
            {
                new OfficeAssignment {
                    ProfessorID = professors.Single( i => i.LastName == "Fakhouri").Id,
                    Location = "Smith 17" },
                new OfficeAssignment {
                    ProfessorID = professors.Single( i => i.LastName == "Harui").Id,
                    Location = "Gowan 27" },
                new OfficeAssignment {
                    ProfessorID = professors.Single( i => i.LastName == "Kapoor").Id,
                    Location = "Thompson 304" },
            };

            foreach (OfficeAssignment o in officeAssignments)
            {
                context.OfficeAssignments.Add(o);
            }
            context.SaveChanges();
            int id = 10;

            var courseInstructors = new CourseAssignment[]
            {
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Kapoor").Id,
                    SemesterID = id

                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Harui").Id,
                    SemesterID = id
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Zheng").Id,
                    SemesterID = id
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Zheng").Id,
                    SemesterID = id
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Fakhouri").Id,
                    SemesterID = id
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Harui").Id,
                    SemesterID = id
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Abercrombie").Id,
                    SemesterID = id
                    },
                new CourseAssignment {
                    CourseID = courses.Single(c => c.Title == "Literature" ).CourseID,
                    ProfessorID = professors.Single(i => i.LastName == "Abercrombie").Id,
                    SemesterID = id

                    },
            };

            foreach (CourseAssignment ci in courseInstructors)
            {
                context.CourseAssignments.Add(ci);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                new Enrollment {
                    SmID = students.Single(s => s.LastName == "Alexander").Id,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID,
                    Grade = Grade.A
                },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Alexander").Id,
                    CourseID = courses.Single(c => c.Title == "Microeconomics" ).CourseID,
                    Grade = Grade.C
                    },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Alexander").Id,
                    CourseID = courses.Single(c => c.Title == "Macroeconomics" ).CourseID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                        SmID = students.Single(s => s.LastName == "Alonso").Id,
                    CourseID = courses.Single(c => c.Title == "Calculus" ).CourseID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                        SmID = students.Single(s => s.LastName == "Alonso").Id,
                    CourseID = courses.Single(c => c.Title == "Trigonometry" ).CourseID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Alonso").Id,
                    CourseID = courses.Single(c => c.Title == "Composition" ).CourseID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Anand").Id,
                    CourseID = courses.Single(c => c.Title == "Chemistry" ).CourseID
                    },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Anand").Id,
                    CourseID = courses.Single(c => c.Title == "Microeconomics").CourseID,
                    Grade = Grade.B
                    },
                new Enrollment {
                    SmID = students.Single(s => s.LastName == "Barzdukas").Id,
                    CourseID = courses.Single(c => c.Title == "Chemistry").CourseID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Li").Id,
                    CourseID = courses.Single(c => c.Title == "Composition").CourseID,
                    Grade = Grade.B
                    },
                    new Enrollment {
                    SmID = students.Single(s => s.LastName == "Justice").Id,
                    CourseID = courses.Single(c => c.Title == "Literature").CourseID,
                    Grade = Grade.B
                    }
            };

            foreach (Enrollment e in enrollments)
            {
                var enrollmentInDataBase = context.Enrollments.Where(
                    s =>
                            s.Student.Id == e.SmID &&
                            s.Course.CourseID == e.CourseID).SingleOrDefault();
                if (enrollmentInDataBase == null)
                {
                    context.Enrollments.Add(e);
                }
            }
            
            context.SaveChanges();

        }
    }
}