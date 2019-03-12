using System.Collections.Generic;

namespace jiraflow.Workflowy
{
    public class Node
    {
        public string Id { get; set; }
        public string Nm { get; set; }
        public string No { get; set; }
        public int Lm { get; set; }
        public int? Cp { get; set; }
        public IEnumerable<Node> Ch { get; set; }
    }
}