using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Constant
{
    public static class NotificationType
    {
        public const string Issue = "Issue";
        public const string AssignCar = "AssignCar";
        public const string StaffManagedBy = "StaffManagedBy";
        public const string CarManagedBy = "CarManagedBy";
        public const string TrainFailed = "TrainFailed";
        public const string TrainSuccess = "TrainSuccess";
        public const string Trainning = "Trainning";
        public const string DeactivedAccount = "Deactivated";
        public const string DeactivedCar = "Deactivated";



        public static string IssueMessage(string name, string type) => "Car " + name + " has new issue " + type + ".";
        public static string AssignCarMessage(string name) => "Car " + name + " assigned to you."; // To Staff
        public static string StaffManagedByManagerMessage(string name) => "Staff " + name + " assigned to you.";
        public static string StaffManagedByStaffMessage(string name) => "From now on, you will be managed by " + name + ".";
        public static string CarManagedByMessage(string name) => "Car " + name + " assigned to you.";
        public static string TrainFailedMessage(string name) => "Model " + name + " train failed.";
        public static string TrainSuccessMessage(string name) => "Model " + name + " train successed.";
        public static string TrainningMessage(string name) => "Model " + name + " is training.";
        public static string DeactivedAccountMessage(string name) => "Staff " + name + " deactivated.";
        public static string DeactivedCarMessage(string name) => "Car " + name + " deactivated";


    }

}
