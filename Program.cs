using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;
using jiraflow.Workflowy;
using Microsoft.Extensions.Configuration;

namespace jiraflow
{
    class Program
    {
        private static string DefaultEpicLink;
        private static Jira jiraClient;
        private static WorkflowyClient workflowyClient;
        private static string projectCode;
        private static string jiraAssignee;

        public static IConfiguration BuildConfiguration() {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);
            var config = configBuilder.Build();
            return config;
        }

        public static async Task Main(string[] args)
        {
            IConfiguration configuration = BuildConfiguration();

            DefaultEpicLink = configuration["defaultEpicLink"];
            jiraClient = Jira.CreateRestClient(configuration["host"], configuration["username"], configuration["password"]);
            workflowyClient = new WorkflowyClient(configuration["host"], configuration["workflowytoken"]);
            projectCode = configuration["projectCode"];
            jiraAssignee = configuration["assignee"];

            try {
                var notes = await workflowyClient.getNodes();

                await createJiraItems(notes, jiraAssignee);
                // await closeCompetedJiraItems(notes);
            }
            catch(Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }


        private async static Task closeCompetedJiraItems(IEnumerable<Note> notes)
        {
            var applicableNodes = findTerm(notes, projectCode)
                .Where(n => n.IsCompletedInWorkflowy);

            var notesToComplete = await applicableNodes.Select(async n => {
                var jiraIssue = await jiraClient.Issues.GetIssueAsync(n.JiraId);
                n.IsCompletedInJira = jiraIssue.IsCompleted();
                if (n.IsCompletedInWorkflowy && !n.IsCompletedInJira) {
                    throw new Exception("Closing issues it not yet supported.");
                    // jiraIssue.s = "Closed";
                    // await jiraIssue.SaveChangesAsync();
                }
                return n;
            })
            .Await();
        }

        private static async Task createJiraItems(IEnumerable<Note> notes, string jiraAssignee)
        {
            IEnumerable<Note> newNotes = findTerm(notes, "#nojira");

            Console.WriteLine($"Found {newNotes.Count()} new workflowy items.");

            IEnumerable<Note> createdJiraItems = await newNotes.Select(async n => {

                try {
                    Issue jiraData = jiraClient.CreateIssue("MR");
                    jiraData.Type = "Task";
                    PopulateJiraIssue(jiraData, n, jiraAssignee);
                    await jiraData.SaveChangesAsync();

                    n.JiraId = jiraData.Key.Value;
                    return n;
                }
                catch ( Exception ex ) 
                {
                    Console.WriteLine($"Ex: {ex.Message} - {n.Text}");
                    return null;
                }
            }).Await();

            await createdJiraItems
                .Where(n => n != null)
                .Select(workflowyClient.Save)
                .Await();
        }

        private static IEnumerable<Note> findTerm(IEnumerable<Note> notes, string term) {
            if ( notes == null ) throw new InvalidOperationException("term param missing");
            return notes.Where(n => (n.Text != null && n.Text.Contains(term)) || (n.Notes != null && n.Notes.Contains(term)));
        }

        private static void PopulateJiraIssue(Issue issue, Note note, string jiraAssignee) {
            string description = note.Notes?.Replace("#nojira", string.Empty);
            string summary = note.Text?.Replace("#nojira", "");
            string epicLink = DefaultEpicLink;

            issue.Assignee = jiraAssignee;
            issue.Description = description;
            issue.Summary = summary;
        }
    }
}
