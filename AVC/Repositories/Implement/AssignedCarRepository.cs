using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class AssignedCarRepository : BaseRepository<AssignedCar>, IAssignedCarRepository
    {
        public AssignedCarRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
