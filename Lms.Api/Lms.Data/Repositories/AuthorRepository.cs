using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Lms.API.Core.Entities;
using Lms.API.Core.Repositories;
using Lms.API.Data.Data;

using Microsoft.EntityFrameworkCore;

namespace Lms.API.Data.Repositories
{
    internal class AuthorRepository : IAuthorRepository
    {
        private readonly LmsAPIContext db;

        public AuthorRepository(LmsAPIContext db)
        {
            this.db = db;
        }

        public async Task AddAsync<T>(T added)
        {
            await db.AddAsync(added);
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await db.Authors.ToListAsync();
        }

        public async Task<Author> GetAuthorAsync(int? id)
        {
            return await db.Authors.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Author>> GetAuthorByNameAsync(string name)
        {
            var names = name.Split(" ");
            var authors = await db.Authors.ToListAsync();
            foreach (string n in names)
            {
                authors = authors.Where(a => a.FirstName.Contains(n, StringComparison.OrdinalIgnoreCase) || a.LastName.Contains(n, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return authors;
        }

        public void Remove(Author removed)
        {
            db.Remove(removed);
        }

        public async Task<bool> SaveAsync()
        {
            return (await db.SaveChangesAsync()) >= 0;
        }

        private void ApplySort(ref IQueryable<Author> authors, string orderByQueryString)
        {
            // Failsafe
            if (!authors.Any())
                return;

            // Defaults to order by Id
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                authors = authors.OrderBy(x => x.Id);
                return;
            }

            // Split up query to individual fields
            var orderParams = orderByQueryString.Trim().Split('&');

            // Collects the properties of Author
            var propertyInfos = typeof(Author).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Checks each parameter in query
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                // Checks param is not empty
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                // Separates Query from property
                var propertyFromQueryName = param.Split("=")[0];

                // Checks properties of Author for matching property
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;
                var sortingOrder = param.EndsWith("desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {sortingOrder}, ");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            if (string.IsNullOrWhiteSpace(orderQuery))
            {
                authors = authors.OrderBy(x => x.FirstName);
                return;
            }

            //authors =  authors.OrderBy(orderQuery);
        }
    }
}