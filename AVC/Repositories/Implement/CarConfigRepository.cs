using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class CarConfigRepository : BaseRepository<CarConfig>, ICarConfigRepository
    {
        public CarConfigRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
