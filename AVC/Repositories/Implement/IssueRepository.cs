using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class IssueRepository : BaseRepository<Issue>, IIssueRepository
    {
        public IssueRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
