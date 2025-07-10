using ayagroup_SMS.Core.Entities;
using ayagroup_SMS.Core.Interfaces.Application.EntityServices;


namespace ayagroup_SMS.Core.Interfaces.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Tasks> Tasks { get; }

        IRepository<Category> Categories { get; }


        Task<int> SaveChangesAsync();
    }
}
