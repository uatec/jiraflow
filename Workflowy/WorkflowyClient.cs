using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jiraflow.Workflowy
{

    internal class WorkflowyClient
    {
        private HttpClient httpClient;

        private Regex jiraRegex = new Regex("[A-Z]+-\\d+");
        private readonly string jiraBaseUrl;

        private string MostRecentOperationTransactionId;
        private int DateJoinedTimestampInSeconds;

        private string extractJiraId(string text) 
        {
            return jiraRegex.Match(text)?.Captures?.SingleOrDefault()?.Value;
        }

        public WorkflowyClient(string jiraBaseUrl, string sessionId)
        {
            this.jiraBaseUrl = jiraBaseUrl;
            var clientHandler = new HttpClientHandler 
            { 
                // Proxy = new WebProxy(new Uri("http://localhost:8888")), 
                // UseProxy = true
            };
            clientHandler.ServerCertificateCustomValidationCallback =  (message, cert, chain, errors) => { return true; };
            this.httpClient = new HttpClient(clientHandler) {
                BaseAddress = new Uri("https://workflowy.com/"),
            };
            this.httpClient.DefaultRequestHeaders.Add("Cookie", $"sessionid={sessionId}");
        }

        public async Task<IEnumerable<Note>> getNodes()
        {
            var response = await httpClient.GetAsync("/get_initialization_data?client_version=19");
            
            response.EnsureSuccessStatusCode();

            var bodyText = await response.Content.ReadAsStringAsync();

            var body = JsonConvert.DeserializeObject<WorkflowyDataResponse>(bodyText);

            this.MostRecentOperationTransactionId = body.ProjectTreeData.MainProjectTreeInfo.InitialMostRecentOperationTransactionId;
            this.DateJoinedTimestampInSeconds = body.ProjectTreeData.MainProjectTreeInfo.DateJoinedTimestampInSeconds;

            return body
                .ProjectTreeData
                .MainProjectTreeInfo
                .RootProjectChildren
                .FlattenChildren(n => n.Ch)
                .Select(n => new Note { 
                    Id = n.Id,
                    Text = n.Nm,
                    Notes = n.No,
                    JiraId = extractJiraId(n.Nm),
                    IsCompletedInWorkflowy = n.Cp != null,
                    LastModified = n.Lm.AsDateTime(this.DateJoinedTimestampInSeconds)
                });
        }

        public async Task Save(Note note)
        {
            int workflowyUserId = 2072725;
            dynamic push_poll_data = new []{new
            {
                most_recent_operation_transaction_id = this.MostRecentOperationTransactionId, // ??
                operations = new dynamic[] 
                {
                    new 
                    {
                        type = "edit",
                        data = new 
                        {
                            projectid = note.Id,
                            name = note.Text.Replace("#nojira", $"#{note.JiraId}"),
                            description = $"{this.jiraBaseUrl}/browse/{note.JiraId}\r\n{note.Notes}"
                        },
                        client_timestamp = note.LastModified.ToTimestamp() - this.DateJoinedTimestampInSeconds,
                        undo_data = new 
                        {
                            previous_last_modified = note.LastModified,
                            previous_name = note.Text,
                            previous_last_modified_by = (string) null
                        }
                    }
                }
            }};

            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(DateTime.Now.ToString("o").Replace("+00:00", "Z")), "client_id");
            content.Add(new StringContent("18"), "client_version");
            content.Add(new StringContent("1234"), "push_poll_id");
            content.Add(new StringContent(JsonConvert.SerializeObject(push_poll_data)), "push_poll_data");
            content.Add(new StringContent(workflowyUserId.ToString()), "crosscheck_user_id");
            
            var response = await this.httpClient.PostAsync("/push_and_poll", content);

            response.EnsureSuccessStatusCode();
        }
    }
}