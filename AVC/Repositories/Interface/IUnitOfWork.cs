
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

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
