using System;

namespace jiraflow.Workflowy
{
    public class Note 
    {
        public string Id { get; set; }
        public string Text { get; set; } = "";
        public string Notes { get; set; } = "";
        public string JiraId { get; set; }
        public string JiraLink { get; set; }
        public bool IsCompletedInWorkflowy { get; set; }
        public bool IsCompletedInJira { get; set; }
        public DateTime LastModified { get; internal set; }
    }
}