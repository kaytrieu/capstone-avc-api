using AVC.Models;

namespace AVC.GenericRepository.Implement
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
