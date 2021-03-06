
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AVC.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        IRoleRepository RoleRepository { get; }
        ICarRepository CarRepository { get; }
        IIssueRepository IssueRepository { get; }
        IModelVersionRepository ModelVersionRepository { get; }
        IIssueTypeRepository IssueTypeRepository { get; }
        IDefaultConfigurationRepository DefaultConfigurationRepository { get; }
        IAssignedCarRepository AssignedCarRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
