using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class DefaultConfigurationRepository : BaseRepository<DefaultConfiguration>, IDefaultConfigurationRepository
    {
        public DefaultConfigurationRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
