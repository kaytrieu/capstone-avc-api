using AutoMapper;
using AVC.Constant;
using AVC.Dtos.DashBoardDtos;
using AVC.Hubs;
using AVC.Repositories.Interface;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AVC.Services.Implements
{
    public class DashBoardService : BaseService, IDashBoardService
    {
        public DashBoardService(IUnitOfWork unit, IMapper mapper, IConfiguration config, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor
            , IHubContext<AVCHub> hubContext) : base(unit, mapper, config, urlHelper, httpContextAccessor, hubContext)
        {
        }

        public DashBoardDto GetDashBoard()
        {
            var dashboard = new DashBoardDto();
            var carList = _unit.CarRepository.GetAll();
            var accountList = _unit.AccountRepository.GetAll();
            var issueList = _unit.IssueRepository.GetAll();

            dashboard.Car = new CarNumber
            {
                Total = carList.Count(),
                Deactivated = carList.Where(x => x.IsAvailable == false).Count()
            };

            dashboard.Manager = new ManagerNumber
            {
                Total = accountList.Where(x => x.RoleId == Roles.ManagerId).Count(),
                Deactivated = accountList.Where(x => x.RoleId == Roles.ManagerId && x.IsAvailable == false).Count()
            };

            dashboard.Staff = new StaffNumber
            {
                Total = accountList.Where(x => x.RoleId == Roles.StaffId).Count(),
                Deactivated = accountList.Where(x => x.RoleId == Roles.StaffId && x.IsAvailable == false).Count()
            };

            dashboard.Issue = new IssueNumber
            {
                Total = issueList.Count(),
                Deactivated = 0
            };

            dashboard.PieChartCar = new PieChartCar
            {
                Total = carList.Count(),
                Connecting = carList.Where(x => x.IsConnecting == true && x.IsApproved == true).Count(),
                Disconnected = carList.Where(x => x.IsConnecting == false && x.IsApproved == true).Count(),
                UnapprovedCount = carList.Where(x => x.IsApproved == null).Count(),
                Rejected = carList.Where(x => x.IsApproved == false).Count()
            };

            var lastWeek = (DateTime.UtcNow.AddHours(7).Date.AddDays(-6));

            var thisWeekIssues = issueList.Where(x => x.CreatedAt >= lastWeek).Include(x => x.Car);

            var carIdGroup = thisWeekIssues.GroupBy(x => new { x.CarId })
                .Select(x => new { Id = x.Key.CarId, Issues = x.Count() })
                .OrderByDescending(x => x.Issues).Take(5).Select(x => x.Id)
                .ToList();



            var group = thisWeekIssues.Where(x => carIdGroup.Contains(x.CarId))
                                      .GroupBy(x => new { x.CarId, x.Car.Name, x.CreatedAt.Date })
                                      .Select(x => new { Id = x.Key.CarId, Name = x.Key.Name, Date = x.Key.Date, Issues = x.Count() }).OrderBy(x => x.Id)
                                      .ToList();

            List<DateTime> thisWeekDays = new List<DateTime>();

            for (int i = 6; i >= 0; i--)
            {
                thisWeekDays.Add(DateTime.UtcNow.AddHours(7).Date.AddDays(-i));
            }

            foreach (var item in group)
            {
                if (dashboard.TopFiveCarIssue.Select(x => x.Id).Contains(item.Id))
                {
                    dashboard.TopFiveCarIssue.Find(x => x.Id == item.Id).Issues[thisWeekDays.IndexOf(item.Date)] = item.Issues;
                }
                else
                {
                    TopFiveCarIssue carTemp = new TopFiveCarIssue();
                    carTemp.Id = item.Id;
                    carTemp.Name = item.Name;
                    carTemp.Issues[thisWeekDays.IndexOf(item.Date)] = item.Issues;
                    dashboard.TopFiveCarIssue.Add(carTemp);

                }
            }


            return dashboard;
        }

    }
}
