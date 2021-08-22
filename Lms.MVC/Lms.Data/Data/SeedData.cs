using System;
using System.Collections.Generic;
using System.Linq;

using Bogus;

using Lms.MVC.Core.Entities;

using Microsoft.AspNetCore.Identity;

namespace Lms.MVC.Data.Data
{
    public class SeedData
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> userManager;

        public int NumberOfCourses { get; set; }

        public int NumberOfModules { get; set; }

        public int NumberOfModulesPerCourse { get; set; }

        public int NumberOfActivities { get; set; }

        public int NumberOfActivititesPerModule { get; set; }

        public int NumberOfStudents { get; set; }

        public int NumberOfStudentsPerClass { get; set; }

        public int NumberOfTeachers { get; set; }

        public int NumberOfTeachersPerClass { get; set; }

        public SeedData(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;

            // Set Random to a fixed number to generate the same data each time Randomizer.Seed =
            // new Random(12345);
            Randomizer.Seed = new Random();

            NumberOfCourses = 11;
            NumberOfModulesPerCourse = 11;
            NumberOfActivititesPerModule = 11;
            NumberOfStudentsPerClass = 11;
            NumberOfTeachersPerClass = 2;

            NumberOfModules = NumberOfCourses * NumberOfModulesPerCourse;
            NumberOfActivities = NumberOfModules * NumberOfActivititesPerModule;
            NumberOfStudents = NumberOfCourses * NumberOfStudentsPerClass;
            NumberOfTeachers = NumberOfCourses;
        }

        public void Seed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            for (int i = 0; i < NumberOfCourses; i++)
            {
                db.Courses.Add(GetCourse(db));
            }
            db.SaveChanges();
        }

        private Course GetCourse(ApplicationDbContext db)
        {
            var fake = new Faker("sv");
            var course = new Course();

            course.Title = fake.Company.CatchPhrase() + " Course";
            course.StartDate = DateTime.Now.AddDays(fake.Random.Int(-2, 2));

            course.Modules = new List<Module>();

            for (int i = 0; i < NumberOfModulesPerCourse; i++)
            {
                course.Modules.Add(GetModule(db));
            }

            course.Users = new List<ApplicationUser>();

            for (int i = 0; i < NumberOfStudentsPerClass; i++)
            {
                course.Users.Add(GetStudent());
            }

            for (int i = 0; i < NumberOfTeachersPerClass; i++)
            {
                course.Users.Add(GetTeacher());
            }

            return course;
        }

        private Module GetModule(ApplicationDbContext db)
        {
            var fake = new Faker("sv");

            var module = new Module();

            module.Title = fake.Name.JobTitle() + " Module";
            module.Description = fake.Lorem.Sentence(10, 5);
            module.StartDate = fake.Date.Soon();
            module.EndDate = module.StartDate.AddDays(fake.Random.Int(4, 30));

            module.Activities = new List<Activity>();
            for (int i = 0; i < NumberOfActivititesPerModule; i++)
            {
                module.Activities.Add(GetActivity(db));
            }

            return module;
        }

        private Activity GetActivity(ApplicationDbContext db)
        {
            var fake = new Faker("sv");

            var ran = fake.Random.Int(1, 5);
            var activity = new Activity();

            activity.Title = fake.Name.JobTitle() + " Activity";
            activity.StartDate = fake.Date.Soon();
            activity.EndDate = activity.StartDate.AddHours(fake.Random.Int(5, 36));
            activity.Description = fake.Lorem.Sentence();
            activity.ActivityType = GetActivityType(ran, db);

            return activity;
        }

        private ApplicationUser GetTeacher()
        {
            var fake = new Faker("sv");

            var teacher = new ApplicationUser();
            var name = fake.Name.FirstName();
            var lastName = fake.Name.LastName();

            //teacher.Name = fake.Name.FullName();
            teacher.Name = name + " " + lastName;
            teacher.Email = fake.Internet.Email(name,lastName); 
            teacher.UserName = teacher.Email;
            teacher.Role = "Teacher";
            teacher.EmailConfirmed = true;

            userManager.CreateAsync(teacher, "password").Wait();
            userManager.AddToRoleAsync(teacher, "Teacher").Wait();

            return teacher;
        }

        private ApplicationUser GetStudent()
        {
            var fake = new Faker("sv");

            var student = new ApplicationUser();
            var name = fake.Name.FirstName();
            var lastName = fake.Name.LastName();

            student.Name = name + " " + lastName;
            student.Email = fake.Internet.Email(name, lastName);
            student.UserName = student.Email;
            student.Role = "Student";
            student.EmailConfirmed = true;

            userManager.CreateAsync(student, "password").Wait();
            userManager.AddToRoleAsync(student, "Student").Wait();

            return student;
        }

        private static ActivityType GetActivityType(int ran, ApplicationDbContext db)

        {
            var result = db.ActivityTypes.Where(a => a.Id == ran).FirstOrDefault();
            return result;
        }
    }
}