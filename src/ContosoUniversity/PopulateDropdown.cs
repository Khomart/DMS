using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using ContosoUniversity.Models.Entities;

namespace ContosoUniversity
{
    public static class PopulateDropdown
    {

        public static SelectList Populate(SchoolContext context, string type, object selected = null, string group = null)
        {
            SelectList list;
            SchoolContext _context = context;
            if (type.Equals("semester"))
            {
                var studentsQuery = from d in _context.Semesters
                                    where (d.Archived == false || d.ID == (int)selected)
                                    orderby d.StartYear
                                    select d;
                //var selectedArchived = _context.Semesters.SingleOrDefault(i => i.ID == (int)selected);
                //if (selectedArchived != null && selectedArchived.Archived == true)
                //    studentsQuery.Append(selectedArchived);
                list = new SelectList(studentsQuery.AsNoTracking(), "ID", "Title", selected, group);
                return list;
            }
            else if (type.Equals("course"))
            {
                var coursesQuery = from d in _context.Courses
                                   where (d.Archived == false || d.CourseID == (int)selected)
                                   orderby d.Title
                                   select d;
                //var selectedArchived = _context.Courses.SingleOrDefault(i => i.CourseID == (int)selected);
                //if (selectedArchived != null && selectedArchived.Archived == true)
                //    coursesQuery.Append(selectedArchived);
                list = new SelectList(coursesQuery, "CourseID", "ShortTitle", selected, group);
                return list;

            }
            else if (type.Equals("department"))
            {
                var departmentQuery = from d in _context.Departments.Include(i => i.Faculty)
                                      where (d.Archived == false || d.DepartmentID == (int)selected)
                                      orderby d.Name
                                      select d;
                //var selectedArchived = _context.Departments.Include(i => i.Faculty).SingleOrDefault(i => i.DepartmentID == (int)selected);
                //if (selectedArchived != null && selectedArchived.Archived == true)
                //    departmentQuery = departmentQuery.add(selectedArchived);
                list = new SelectList(departmentQuery, "DepartmentID", "Name", selected, group);
                return list;

            }
            else if (type.Equals("program"))
            {
                var programQuery = from d in _context.Programs.Include(i => i.Department)
                                      where (d.Archived == false || d.ProgramID == (int)selected)
                                      orderby d.Title
                                      select d;
                //var selectedArchived = _context.Programs.SingleOrDefault(i => i.ProgramID == (int)selected);
                //if (selectedArchived != null && selectedArchived.Archived == true)
                //    programQuery.Append(selectedArchived);
                list = new SelectList(programQuery, "ProgramID", "Title", selected, group);
                return list;

            }
            else if (type.Equals("professor"))
            {
                var professorQuery = from d in _context.Professors
                                   where (d.Archived == false || d.Id == (int)selected)
                                     orderby d.LastName
                                   select d;
                //var selectedArchived = _context.Professors.SingleOrDefault(i => i.Id == (int)selected);
                //if (selectedArchived != null && selectedArchived.Archived == true)
                //    professorQuery.Append(selectedArchived);
                list = new SelectList(professorQuery, "Id", "FullName");
                return list;

            }
            else return null;
        }
    }
}
