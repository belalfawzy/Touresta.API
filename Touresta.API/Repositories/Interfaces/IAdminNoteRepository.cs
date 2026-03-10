using RAFIQ.API.Models;

namespace RAFIQ.API.Repositories.Interfaces
{
    public interface IAdminNoteRepository
    {
        IQueryable<AdminNote> Query();
        void Add(AdminNote note);
        Task SaveChangesAsync();
    }
}