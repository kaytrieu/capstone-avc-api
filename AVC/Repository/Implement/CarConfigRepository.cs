using AVC.Models;

namespace AVC.GenericRepository.Implement
{
    public class CarConfigRepository : BaseRepository<CarConfig>, ICarConfigRepository
    {
        public CarConfigRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
