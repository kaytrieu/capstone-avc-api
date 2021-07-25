using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Dtos.DashBoardDtos
{
    public class DashBoardDto
    {
        public CarNumber Car { get; set; }
        public ManagerNumber Manager { get; set; }
        public StaffNumber Staff { get; set; }
        public IssueNumber Issue { get; set; }
        public List<TopFiveCarIssue> TopFiveCarIssue { get; set; }
        public PieChartCar PieChartCar { get; set; }

        public DashBoardDto()
        {
            Car =  new CarNumber();
            Manager =  new ManagerNumber();
            Staff = new StaffNumber();
            Issue = new IssueNumber();
            TopFiveCarIssue = new List<TopFiveCarIssue>();
            PieChartCar = new PieChartCar();
        }
    }

    public class CarNumber
    {
        public int Total { get; set; }
        public int Deactivated { get; set; }
    }

    public class ManagerNumber
    {
        public int Total { get; set; }
        public int Deactivated { get; set; }
    }

    public class StaffNumber
    {
        public int Total { get; set; }
        public int Deactivated { get; set; }
    }

    public class IssueNumber
    {
        public int Total { get; set; }
        public int Deactivated { get; set; }
    }

    public class TopFiveCarIssue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Issues { get; set; } = new int[] { 0, 0, 0, 0, 0, 0, 0 };
    }

    public class PieChartCar
    {
        public int Total { get; set; }
        public int UnapprovedCount { get; set; }
        public int Connecting { get; set; }
        public int Disconnected { get; set; }
        public int Rejected { get; set; }
    }
}
