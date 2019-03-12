namespace jiraflow.Workflowy
{
    public class ProjectTreeData 
    {
        public string ClientId { get; set; }
        public MainProjectTreeInfo MainProjectTreeInfo { get; set; }
        public dynamic[] AuxiliaryProjectTreeInfos { get; set; }
    }
}