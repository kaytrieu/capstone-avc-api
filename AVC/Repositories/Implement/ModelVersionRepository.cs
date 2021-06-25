using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class ModelVersionRepository : BaseRepository<ModelVersion>, IModelVersionRepository
    {
        public ModelVersionRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
