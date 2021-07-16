using AVC.Models;
using AVC.Repositories.Interface;

namespace AVC.GenericRepository.Implement
{
    public class UserNotificationRepository : BaseRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(AVCContext dbContext) : base(dbContext)
        {
        }

    }
}
