using AVC.Models;

namespace AVC.GenericRepository.Implement
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
