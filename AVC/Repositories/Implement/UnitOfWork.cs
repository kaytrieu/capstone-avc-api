
using AVC.GenericRepository.Implement;
using AVC.Models;
using AVC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace AVC.Repositories.Implement
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AVCContext _context;
        private readonly IRoleRepository _roleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICarRepository _carRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IModelVersionRepository _modelVersionRepository;
        private readonly IIssueTypeRepository _issueTypeRepository;
        private readonly IDefaultConfigurationRepository _defaultConfigurationRepository;
        private readonly IAssignedCarRepository _assignedCarRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        public UnitOfWork(AVCContext context)
        {
            _context = context;
        }

        public IRoleRepository RoleRepository => _roleRepository ?? new RoleRepository(_context);
        public IAccountRepository AccountRepository => _accountRepository ?? new AccountRepository(_context);

        public ICarRepository CarRepository => _carRepository ?? new CarRepository(_context);


        public IIssueRepository IssueRepository => _issueRepository ?? new IssueRepository(_context);

        public IModelVersionRepository ModelVersionRepository => _modelVersionRepository ?? new ModelVersionRepository(_context);
        public IIssueTypeRepository IssueTypeRepository => _issueTypeRepository ?? new IssueTypeRepository(_context);

        public IDefaultConfigurationRepository DefaultConfigurationRepository => _defaultConfigurationRepository ?? new DefaultConfigurationRepository(_context);
        public IAssignedCarRepository AssignedCarRepository => _assignedCarRepository ?? new AssignedCarRepository(_context);
        public IUserNotificationRepository UserNotificationRepository => _userNotificationRepository ?? new UserNotificationRepository(_context);

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
