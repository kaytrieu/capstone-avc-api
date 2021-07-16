using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Constant
{
    public static class NotificationType
    {
        public const string Issue = "Issue";
        public const string AssignCar = "Assign Car";
        public const string StaffManagedBy = "Staff Managed By";
        public const string CarManagedBy = "Car Managed By";
        public const string TrainFailed = "Train Failed";
        public const string TrainSuccess = "Train Success";
        public const string Trainning = "Trainning";
        public const string DeactivatedAccount = "Deactivated Account";
        public const string DeactivatedCar = "Deactivated Car";



        public static string IssueMessage(string name, string type) => "The car " + name + " has just got an " + type + " issue, please check for details.";
        public static string AssignCarOldStaffMessage(string name) => "A car " + name + " has been removed from you, please check for detail.";// To Staff
        public static string AssignCarNewStaffMessage(string name) => "You have just been assigned a car from your manager, please check for details"; // To Staff
        public static string StaffManagedByStaffMessage(string name) => "You have been assigned to manager " + name + ".";
        public static string StaffManagedByNewManagerMessage(string name) => "Staff " + name + " has been assigned to you, please check for detail.";
        public static string StaffManagedByOldManagerMessage(string name) => "Staff " + name + " has been removed from you, please check for detail.";
        public static string CarManagedByNewManagerMessage(string name) => "A car " + name + " has been assigned to you, please check for detail.";
        public static string CarManagedByOldManagerMessage(string name) => "A car " + name + " has been removed from you, please check for detail.";
        public static string TrainFailedMessage(string name, string message) => $"Model {name} train failed with message: {message}.";
        public static string TrainSuccessMessage(string name) => "Model " + name + " train succeeded.";
        public static string TrainningMessage(string name) => "Model " + name + " is training.";
        public static string DeactivatedAccountManagerMessage(string name) => "Staff " + name + " has been deactivated and removed from your management list.";
        public static string DeactivatedAccountStaffMessage(string name) => "Manager " + name + " has been deactivated, any car assigned to you by that account will be removed.";
        public static string DeactivatedCarMessage(string name) => "Car " + name + " has been deactivated and removed from your list";


    }

}
