using ayagroup_SMS.Core.Interfaces.Infrastructure;
using ayagroup_SMS.Infrastructure.Data;
using ayagroup_SMS.Core.Entities;

namespace ayagroup_SMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        private IRepository<User> _users;
        public IRepository<User> Users => _users ??= new Repository<User>(_context);


        private IRepository<Tasks> _tasks;
        public IRepository<Tasks> Tasks => _tasks = new Repository<Tasks>(_context);

        private IRepository<Category> _categorey;
        public IRepository<Category> Categories => _categorey ??= new Repository<Category>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
