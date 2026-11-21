using System.Collections.Generic;

namespace CampusEventManagement
{
    public class ModuleConfig
    {
        public string Title, Table, OwnerColumn;
        public string[] Fields;

        public ModuleConfig(string title, string table, string owner, params string[] fields)
        {
            Title = title;
            Table = table;
            OwnerColumn = owner;
            Fields = fields;
        }
    }

    public static class Modules
    {
        public static List<ModuleConfig> ForRole(string role)
        {
            var x = new List<ModuleConfig>();

            if (role == "Admin")
            {
                x.Add(new ModuleConfig("User Management", "Users", null,
                    "FullName", "Username", "Email", "Role", "Phone", "Department", "IsActive"));
                x.Add(new ModuleConfig("Category Management", "Categories", null,
                    "CategoryName", "Description"));
                x.Add(new ModuleConfig("Venue Management", "Venues", null,
                    "VenueName", "Location", "Capacity", "IsAvailable"));
                x.Add(new ModuleConfig("Notice Management", "Notices", null,
                    "Title", "Body"));
            }
            else if (role == "Organizer")
            {
                x.Add(new ModuleConfig("Event Management", "Events", "OwnerUserId",
                    "Title", "Description", "Category", "Venue", "EventDate", "Capacity", "Status"));
                x.Add(new ModuleConfig("Schedule Management", "Schedules", "OwnerUserId",
                    "EventTitle", "SessionTitle", "StartTime", "EndTime", "Room"));
                x.Add(new ModuleConfig("Speaker Management", "Speakers", "OwnerUserId",
                    "EventTitle", "SpeakerName", "Email", "Topic"));
                x.Add(new ModuleConfig("Sponsor Management", "Sponsors", "OwnerUserId",
                    "EventTitle", "SponsorName", "ContactEmail", "Contribution"));
            }
            else if (role == "Student")
            {
                x.Add(new ModuleConfig("Event Registration", "Registrations", "OwnerUserId",
                    "EventTitle", "StudentName", "StudentEmail", "RegistrationStatus"));
                x.Add(new ModuleConfig("Feedback Management", "Feedback", "OwnerUserId",
                    "EventTitle", "Rating", "Comment"));
                x.Add(new ModuleConfig("Favourite Events", "Favourites", "OwnerUserId",
                    "EventTitle", "Note"));
                x.Add(new ModuleConfig("Participation Management", "Participations", "OwnerUserId",
                    "EventTitle", "AttendanceStatus", "CertificateStatus"));
            }
            else if (role == "Faculty")
            {
                x.Add(new ModuleConfig("Event Approval", "Approvals", "OwnerUserId",
                    "EventTitle", "OrganizerName", "Decision", "Remarks"));
                x.Add(new ModuleConfig("Department Event Management", "DepartmentEvents", "OwnerUserId",
                    "Department", "EventTitle", "EventDate", "Coordinator"));
                x.Add(new ModuleConfig("Resource Management", "Resources", "OwnerUserId",
                    "EventTitle", "ResourceName", "Quantity", "ResourceStatus"));
                x.Add(new ModuleConfig("Event Evaluation", "Evaluations", "OwnerUserId",
                    "EventTitle", "EvaluationScore", "Strengths", "Improvements"));
            }

            return x;
        }
    }
}
