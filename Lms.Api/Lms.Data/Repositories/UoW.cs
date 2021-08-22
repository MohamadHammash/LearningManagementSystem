using System.Threading.Tasks;

using Lms.API.Core.Repositories;
using Lms.API.Data.Data;

namespace Lms.API.Data.Repositories
{
    public class UoW : IUoW
    {
        private readonly LmsAPIContext db;

        public IPublicationRepository PublicationRepository { get; }

        public IAuthorRepository AuthorRepository { get; }

        public UoW(LmsAPIContext db)
        {
            this.db = db;
            PublicationRepository = new PublicationRepository(db);
            AuthorRepository = new AuthorRepository(db);
        }

        public async Task CompleteAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}