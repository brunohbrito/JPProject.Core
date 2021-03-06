using JPProject.Domain.Core.Interfaces;
using JPProject.EntityFrameworkCore.Interfaces;
using System.Threading.Tasks;

namespace JPProject.EntityFrameworkCore.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IJpEntityFrameworkStore _context;
        private readonly IEventStoreContext _eventStoreContext;

        public UnitOfWork(IJpEntityFrameworkStore context, IEventStoreContext eventStoreContext)
        {
            _context = context;
            _eventStoreContext = eventStoreContext;
        }

        public async Task<bool> Commit()
        {
            var linesModified = await _context.SaveChangesAsync();
            if (_eventStoreContext.GetType() != _context.GetType())
                await _eventStoreContext.SaveChangesAsync();
            return linesModified > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
            _eventStoreContext.Dispose();
        }
    }
}
