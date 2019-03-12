namespace jiraflow.Workflowy
{
    public class WorkflowyDataResponse 
    {
        public ProjectTreeData ProjectTreeData { get; set; }
        public dynamic User { get; set; }
        public dynamic Global { get; set; }
        public dynamic Settings { get; set; }
        public dynamic Features { get; set; }
    }
}