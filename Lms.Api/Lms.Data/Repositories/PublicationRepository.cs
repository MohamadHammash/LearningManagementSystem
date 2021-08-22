using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lms.API.Core.Entities;
using Lms.API.Core.Repositories;
using Lms.API.Data.Data;

using Microsoft.EntityFrameworkCore;

namespace Lms.API.Data.Repositories
{
    internal class PublicationRepository : IPublicationRepository
    {
        private readonly LmsAPIContext db;

        public PublicationRepository(LmsAPIContext db)
        {
            this.db = db;
        }

        public async Task AddAsync<T>(T added)
        {
            await db.AddAsync(added);
        }

        public async Task<IEnumerable<Publication>> GetAllPublicationsAsync()
        {
            return await db.Publications.Include(l => l.Subject).Include(l => l.Level).Include(a => a.Authors).ToListAsync();
        }

        public async Task<Publication> GetPublicationAsync(int? id)
        {
            return await db.Publications.Include(l => l.Subject).Include(l => l.Level).FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Publication>> GetPublicationByTitleAsync(string title)
        {
            return await db.Publications.Include(l => l.Subject).Include(l => l.Level).Where(l => l.Title.Contains(title)).ToListAsync();
        }

        public async Task<IEnumerable<Publication>> GetPublicationBySearchAsync(string search)
        {
            string[] searchSplit = (search.First() == '"' && search.Last() == '"') ?
                new string[] { search[1..^1] } :
                search.Split(' ');
            var result = await db.Publications
                .Include(l => l.Subject).Include(l => l.Level).Include(l => l.Authors).ToListAsync();

            foreach (string word in searchSplit)
            {
                result = result.Where(l => l.Title.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                    l.Subject.Title.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                    l.Authors.Where(a => a.FirstName.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                    a.LastName.Contains(word, StringComparison.OrdinalIgnoreCase)).Count() > 0).ToList();
            }

            return result;
        }

        public void Remove(Publication removed)
        {
            db.Remove(removed);
        }

        public async Task<bool> SaveAsync()
        {
            return (await db.SaveChangesAsync()) >= 0;
        }

        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
            await db.DisposeAsync();
        }
    }
}