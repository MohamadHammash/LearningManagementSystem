using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bogus;

using Lms.API.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lms.API.Data.Data
{
    public class SeedData
    {
        public static async Task InitAsync(IServiceProvider services)
        {
            using (var db = new LmsAPIContext(services.GetRequiredService<DbContextOptions<LmsAPIContext>>()))
            {
                var fake = new Faker("sv");

                if (db.Authors.Any())
                {
                    return;
                }

                List<Level> levels;
                if (!db.Levels.Any())
                {
                    levels = GetLevels();
                    await db.AddRangeAsync(levels);
                }
                else
                    levels = await db.Levels.ToListAsync();

                var subjects = GetSubjects();

                var authors = GetAuthors();
                var publications = GetPublications(authors, levels, subjects);
                ConnectAuthorsWithPublications(authors, publications);

                await db.AddRangeAsync(authors);
                await db.AddRangeAsync(publications);
                await db.AddRangeAsync(subjects);

                await db.SaveChangesAsync();
            };
        }

        private static void ConnectAuthorsWithPublications(List<Author> authors, List<Publication> publications)
        {
            var fake = new Faker("sv");

            foreach (Publication lit in publications)
            {
                var authorInsert = new List<Author>();
                for (int j = 0; j < fake.Random.Int(1, 3); j++)
                {
                    int r = fake.Random.Int(0, authors.Count - 1);
                    if (!authorInsert.Contains(authors[r]))
                        authorInsert.Add(authors[r]);
                }
                lit.Authors = authorInsert;
            }
        }

        private static List<Publication> GetPublications(List<Author> authors, List<Level> levels, List<Subject> subjects)
        {
            var fake = new Faker("sv");
            var publication = new List<Publication>();

            for (int i = 0; i < 30; i++)
            {
                var lit = new Publication
                {
                    Title = fake.Company.CompanyName(),
                    Description = fake.Lorem.Paragraph(),
                    ReleaseDate = fake.Date.Between(DateTime.Now.AddYears(-30), DateTime.Now.AddMonths(-3)),
                    Url = fake.Internet.Url(),
                    Subject = subjects[fake.Random.Int(0, subjects.Count - 1)],
                    Authors = new List<Author>()
                };
                publication.Add(lit);
            }

            return publication;
        }

        private static List<Author> GetAuthors()
        {
            var fake = new Faker("sv");
            var authors = new List<Author>();
            for (int i = 0; i < 20; i++)
            {
                var author = new Author
                {
                    FirstName = fake.Name.FirstName(),
                    LastName = fake.Name.LastName(),
                    DateOfBirth = fake.Date.Between(DateTime.Today.AddYears(-100), DateTime.Today.AddYears(-20))
                };
                authors.Add(author);
            }
            return authors;
        }

        private static List<Subject> GetSubjects()
        {
            var fake = new Faker("sv");
            var subjects = new List<Subject>();
            for (int i = 0; i < 10; i++)
            {
                subjects.Add(new Subject
                {
                    Title = fake.Commerce.Product()
                });
            }
            return subjects;
        }

        private static List<Level> GetLevels()
        {
            var levels = new List<Level>
            {
                new Level { Name = "Beginner" },
                new Level { Name = "Intermediate" },
                new Level { Name = "Advanced" }
            };

            return levels;
        }
    }
}