using Touresta.API.Models;

namespace Touresta.API.Repositories.Interfaces
{
    public interface IAdminNoteRepository
    {
        IQueryable<AdminNote> Query();
        void Add(AdminNote note);
        Task SaveChangesAsync();
    }
}