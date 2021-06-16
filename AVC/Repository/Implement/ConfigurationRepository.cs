using AVC.Models;

namespace AVC.GenericRepository.Implement
{
    public class ConfigurationRepository : BaseRepository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
