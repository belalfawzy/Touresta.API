using Touresta.API.Data;
using Touresta.API.Models;
using Touresta.API.Repositories.Interfaces;

namespace Touresta.API.Repositories.Implementations
{
    public class AdminNoteRepository : IAdminNoteRepository
    {
        private readonly AppDbContext _db;

        public AdminNoteRepository(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<AdminNote> Query() => _db.AdminNotes.AsQueryable();

        public void Add(AdminNote note) => _db.AdminNotes.Add(note);

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}