using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class IssueTypeRepository : BaseRepository<IssueType>, IIssueTypeRepository
    {
        public IssueTypeRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
