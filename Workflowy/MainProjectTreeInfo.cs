using System.Collections.Generic;

namespace jiraflow.Workflowy
{
    public class MainProjectTreeInfo 
    {
        public dynamic RootProject { get; set; }
        public IEnumerable<Node> RootProjectChildren { get; set; }
        public string InitialMostRecentOperationTransactionId { get; set; }
        public int initialPollingIntervalInMs { get; set; }
        public dynamic[] ServerExpandedProjectsList { get; set; }
        public bool IsReadOnly { get; set; }
        public int OwnerId { get; set; }
        public int DateJoinedTimestampInSeconds { get; set; }
        public int ItemsCreatedInCurrentMonth { get; set; }
        public int MonthlyItemQuota { get; set; }
    }
}