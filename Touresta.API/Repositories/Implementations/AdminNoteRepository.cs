using RAFIQ.API.Data;
using RAFIQ.API.Models;
using RAFIQ.API.Repositories.Interfaces;

namespace RAFIQ.API.Repositories.Implementations
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