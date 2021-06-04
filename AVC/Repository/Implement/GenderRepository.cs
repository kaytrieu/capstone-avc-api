using AVC.Models;

namespace AVC.GenericRepository.Implement
{
    public class GenderRepository : BaseRepository<Gender>, IGenderRepository
    {
        public GenderRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
