﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

using YouTrackSharp;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace Email_to_YouTrack
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Update settings if app version changed.
            // https://stackoverflow.com/questions/534261/how-do-you-keep-user-config-settings-across-different-assembly-versions-in-net/534335#534335
            if (Properties.Settings.Default.upgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.upgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }


        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        public async Task FromMail(Outlook.MailItem mail)
        {
            var project = Properties.Settings.Default.project;
            var connection = new YouTrackSharp.BearerTokenConnection(Properties.Settings.Default.baseUrl, Properties.Settings.Default.perm);
            var issuesService = new YouTrackSharp.Issues.IssuesService(connection);

            var projID = await this.GetProjectId(connection, project);
            var str = await this.CreateIssue(connection, projID, mail.Subject, mail.Body);
        }

        public async Task<string> GetProjectId(YouTrackSharp.Connection connection, string projectName)
        {
            var client = await connection.GetAuthenticatedHttpClient();
            var query = "api/admin/projects?fields=id,name,shortName&query=" + WebUtility.UrlEncode(projectName);
            var response = await client.GetAsync(query);
            
            if (response.IsSuccessStatusCode)
            {
                var projs = JsonConvert.DeserializeObject<List<JsonProject>>(response.Content.ReadAsStringAsync().Result);
                if (projs.Count == 1)
                {
                    return projs[0].id;
                }
                throw new ApplicationException("Unable to find unique project matching \"" + projectName + "\". Server returned " + projs.Count + " projects.");
            }
            else
            {
                throw new ApplicationException("Query \"" + query + "\" failed with code " + response.StatusCode + " and message:\n" + response.Content.ReadAsStringAsync().Result);
            }
        }

        public async Task<string> CreateIssue(YouTrackSharp.Connection _connection, string projectId, string summary, string description)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            var json = new JsonIssue();
            json.project.id = projectId;
            json.summary = summary;
            json.description = description;
            json.customFields.Add(new JsonCustomField("Priority", "SingleEnumIssueCustomField", "Critical"));
            
            var client = await _connection.GetAuthenticatedHttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/issues?fields=idReadable", content);

            response.EnsureSuccessStatusCode();

            var resp = JsonConvert.DeserializeObject<JsonIssue>(await response.Content.ReadAsStringAsync());
            var issueId = resp.idReadable;

            var uri = new Uri(new Uri(Properties.Settings.Default.baseUrl), "issue/" + issueId).ToString();
            System.Diagnostics.Process.Start(uri);

            return issueId;
        }

    #region VSTO generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }

    public class JsonProject
    {
        public string shortName;
        public string name;
        public string id;
        [JsonProperty("$type")]
        public string t;
    }

    /* {
            "project": {"id": "0-0"},
            "summary":"REST API lets you create issues!",
            "description":"Let'\''s create a new issue using YouTrack'\''s REST API."
            "customFields": [
                {
                    "value": {"name": "Show-stopper"},
                    "name": "Priority",
                    "$type": "SingleEnumIssueCustomField"
                },
                {
                    "value": {"name": "Task"},
                    "name": "Type",
                    "$type": "SingleEnumIssueCustomField"
                },
                {
                    "value": {"name": "Open"},
                    "name": "State",
                    "$type": "StateIssueCustomField"
                },
                {
                    "value": {"login": "jane.doe"},
                    "name": "Assignee",
                    "$type": "SingleUserIssueCustomField"
                },
            ],
        }*/
    public class JsonIssue
    {
        public string idReadable;
        public JsonProject project = new JsonProject();
        public string summary;
        public string description;
        public List<JsonCustomField> customFields = new List<JsonCustomField>();
    }

    public class JsonCustomField
    {
        public JsonCustomField(string name, string t, string value)
        {
            this.name = name;
            this.t = t;
            this.value.name = value;
        }

        public string name;
        [JsonProperty("$type")]
        public string t;
        public JsonCustomFieldValue value = new JsonCustomFieldValue();
    }
    public class JsonCustomFieldValue
    {
        public string name;
        [JsonProperty("$type", NullValueHandling = NullValueHandling.Ignore)]
        public string t;
        public string login;
    }
}
