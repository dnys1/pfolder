using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace pfolder
{
    public partial class ProjectToolForm : Form
    {
        /// <summary>
        /// Used to retrieve the HTML for the binder tab in PDS.
        /// </summary>
        private string _binderURL;

        /// <summary>
        /// Used to retrieve a project's SID given its project number.
        /// </summary>
        private string _SIDlookupURL;

        /// <summary>
        /// Used to retrieve the user's NTID.
        /// </summary>
        private string _homeHeaderURL;

        /// <summary>
        /// The BC PDS homepage.
        /// </summary>
        private string _homeURL;

        /// <summary>
        /// User cookie stored for calls to PDS website.
        /// </summary>
        private string _cookie;

        public string Cookie
        {
            set
            {
                _cookie = value;
            }
        }

        private bool _testing;

        /// <summary>
        /// User NTID stored for calls to PDS website.
        /// </summary>
        // private string _NTID;

        private bool _isMessageBoxShowing = false;

        private enum MessageType
        {
            Warning,
            Error,
            Info
        }

        public static string CONFIDENTIAL = "CONFIDENTIAL";
        
        public static string UNAVAILABLE = "UNAVAILABLE";

        public ProjectToolForm(bool testing = false)
        {
            _testing = testing;
            LoadStrings();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetForm();
        }

        private async void Button_ProjectFolder_Click(object sender, EventArgs e)
        {
            await LoadProjectFolder();
        }

        private async void Button_BD_Click(object sender, EventArgs e)
        {
            await LoadProjectFolder(isBD: true);
        }

        private void ResetForm(bool resetTxtBox = true)
        {
            if (!Created)
            {
                return;
            }

            if (resetTxtBox)
            {
                ProjectNo_TextBox.Text = "";
            }
            ProjectNo_TextBox.Focus();
            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = 100;
            ProgressBar.Value = 0;
        }
        
        private void ShowError(string error, MessageType type)
        {
            if (_testing)
            {
                return;
            }

            MessageBoxIcon icon = MessageBoxIcon.None;
            string caption = "";
            if (type == MessageType.Warning)
            {
                icon = MessageBoxIcon.Warning;
                caption = "Warning";
            }
            else if (type == MessageType.Error)
            {
                icon = MessageBoxIcon.Error;
                caption = "Error";
            }
            if (Created)
            {
                _isMessageBoxShowing = true;
                MessageBox.Show(error, caption, MessageBoxButtons.OK, icon);
                _isMessageBoxShowing = false;
            } else
            {
                LogStatus(string.Concat(caption, ": ", error), type);
            }
        }

        private void LogStatus(string error, MessageType type = MessageType.Info)
        {
            if (_testing)
            {
                return;
            }
#if DEBUG
            Console.WriteLine(string.Concat(type.ToString(), ": ", error));
#endif
        }

        public async Task<string> LoadProjectFolder(string projectNo = null, bool isBD = false)
        {
            if (projectNo == null)
            {
                projectNo = ProjectNo_TextBox.Text;
            }
            if (projectNo.Length == 0)
            {
                return null;
            }

            Regex projectNoMatcher = new Regex(@"^\d\d\d\d\d\d$");
            if (!projectNoMatcher.IsMatch(projectNo))
            {
                string error = "BC Project No. must be exactly 6 digits.";
                ShowError(error, MessageType.Warning);

                ResetForm(false);

                return null;
            }

            if (_cookie == null)
            {
                LogStatus("Retrieving cookie from server...");

                _cookie = await GetCookie();
                
                LogStatus(string.Concat("Cookie retrieved: ", _cookie));
            }
            
            LogStatus(string.Concat("Retrieving SID for Project ", projectNo));

            string SID = await GetSID(projectNo);
            
            LogStatus(string.Concat("SID Retrieved: ", SID));

            string path;
            bool jsonChecked = false;
            // If nothing came back from request, return without continuing
            if (SID == null)
            {
                LogStatus("SID retrieved is null. Program exiting.");

                string error = "An unknown error occurred. Please try again later.";
                ShowError(error, MessageType.Error);
                ResetForm();
                return null;
            }
            // Project does not exist on PDS... check JSON tree
            else if (SID == projectNo)
            {
                LogStatus("SID retrieved is same as project number. Loading json...");

                if (!isBD)
                {
                    path = CheckJsonForProject(projectNo);
                    jsonChecked = true;
                    goto openFolder;
                } else
                {
                    string error = "Project does not have a BD folder on record.";
                    ShowError(error, MessageType.Warning);
                    ResetForm(resetTxtBox: false);
                    return UNAVAILABLE;
                }
            }

            ProgressBar.Value = 25;
            string binderHTML = await GetBinderHTML(SID);

            // If nothing came back from request, return without continuing
            if (binderHTML == null)
            {
                LogStatus("Binder HTML is null. Program exiting.");

                ResetForm();
                return null;
            }

            ProgressBar.Value = 75;
            path = GetProjectPath(binderHTML, isBD);
            
            LogStatus(string.Concat("Project path retrieved: ", path));

        openFolder:
            if (path == null)
            {
                string error = "An unknown error occurred. Please try again.";
                ShowError(error, MessageType.Error);

                ResetForm();
                return null;
            }
            else if (path == CONFIDENTIAL)
            {
                string error = "Profile is confidential. Please visit the profile on PDS to request access.";
                ShowError(error, MessageType.Warning);
                ResetForm(resetTxtBox: false);

                return CONFIDENTIAL;
            }
            else if (path == UNAVAILABLE)
            {

                if (isBD)
                {
                    string error = "Project does not have a BD folder on record.";
                    ShowError(error, MessageType.Warning);
                    ResetForm(resetTxtBox: false);
                }

                if (!jsonChecked)
                {
                    path = CheckJsonForProject(projectNo);
                    jsonChecked = true;
                }

                if (path == UNAVAILABLE)
                {
                    LogStatus("Project unavailable on PDS or JSON. Program exiting.");

                    string error = "Unable to locate project. Please check the number and try again.";
                    ShowError(error, MessageType.Warning);
                    ResetForm(resetTxtBox: false);

                    return UNAVAILABLE;
                }

                goto openFolder;
            }

            if (!_testing)
            {
                ProgressBar.Value = 100;
                if (path.IndexOf("file:") != -1)
                {
                    LogStatus("Opening... " + path);

                    try
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    catch (Exception)
                    {
                        string error = "The folder path on record is no longer valid.";
                        ShowError(error, MessageType.Error);
                    }
                    ResetForm(false);
                }
                else
                {
                    string error = "An unknown error occurred. Please try again.";
                    ShowError(error, MessageType.Error);
                    ResetForm();
                }

                Close();
            }

            return path;
        }

        public async Task<string> GetNTID()
        {
            LogStatus("Retrieving user's NTID...");

            WebRequest WReq = WebRequest.Create(_homeHeaderURL);
            WReq.Headers.Add(HttpRequestHeader.Cookie, _cookie);
            WReq.Credentials = CredentialCache.DefaultCredentials;

            LogStatus("----NTID WebRequest created.");

            WebResponse WResp = null;
            try
            {
                WResp = await WReq.GetResponseAsync();
            } catch (Exception e)
            {
                string error = e.Message;
                ShowError(error, MessageType.Error);
            }

            if (WResp == null)
            {
                return null;
            }

            LogStatus("----NTID WebResponse received.");

            if (((HttpWebResponse)WResp).StatusCode == HttpStatusCode.OK)
            {
                string response;
                using (Stream dataStream = WResp.GetResponseStream())
                {
                    // Open the Stream using a StreamReader for easy access
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content
                    response = reader.ReadToEnd();
                }
                WResp.Close();

                // Parse the retrieved HTML for the user's NTID
                HtmlAgilityPack.HtmlDocument NTIDdoc = new HtmlAgilityPack.HtmlDocument();

                NTIDdoc.LoadHtml(response);

                HtmlNode node = NTIDdoc.GetElementbyId("myNTID");

                if (node == null)
                {
                    return null;
                }

                return node.GetAttributeValue("value", null);
            }

            return null;
        }

        public async Task<string> GetCookie()
        {
            WebRequest WReq = WebRequest.Create(_homeURL);
            WReq.Credentials = CredentialCache.DefaultCredentials;

            LogStatus("----Cookie WebRequest created.");

            WebResponse WResp = null;
            try
            {
                WResp = await WReq.GetResponseAsync();
            }
            catch (Exception e)
            {
                string error = e.Message;
                ShowError(error, MessageType.Error);
            }

            if (WResp == null)
            {
                return null;
            }

            LogStatus("----Cookie WebResponse recevied.");

            return WResp.Headers.Get("Set-Cookie").Split(';')[0];
        }

        public async Task<string> GetSID(string projectNo)
        {
            WebRequest WReq = WebRequest.Create(_SIDlookupURL + projectNo);
            WReq.Headers.Add(HttpRequestHeader.Cookie, _cookie);
            WReq.Credentials = CredentialCache.DefaultCredentials;

            LogStatus("----SID WebRequest created.");

            WebResponse WResp = null;
            try
            {
                WResp = await WReq.GetResponseAsync();
            }
            catch (Exception e)
            {
                ShowError(e.Message, MessageType.Error);
            }

            if (WResp == null)
            {
                return null;
            }

            LogStatus("----SID WebResponse received.");

            if (((HttpWebResponse)WResp).StatusCode == HttpStatusCode.OK)
            {
                string response;
                using (Stream dataStream = WResp.GetResponseStream())
                {
                    // Open the Stream using a StreamReader for easy access
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content
                    response = reader.ReadToEnd();
                }
                WResp.Close();

                return response.Split('|')[0];
            }

            string error = ((HttpWebResponse)WResp).StatusDescription;
            ShowError(error, MessageType.Error);

            return null;
        }

        public async Task<string> GetBinderHTML(string SID)
        {
            WebRequest WReq = WebRequest.Create(_binderURL + SID);
            WReq.Headers.Add(HttpRequestHeader.Cookie, _cookie);
            WReq.Credentials = CredentialCache.DefaultCredentials;

            WebResponse WResp = null;
            try
            {
                WResp = await WReq.GetResponseAsync();
            }
            catch (Exception e)
            {
                ShowError(e.Message, MessageType.Error);
            }

            if (WResp == null)
            {
                return null;
            }

            if (((HttpWebResponse)WResp).StatusCode == HttpStatusCode.OK)
            {
                string response;
                using (Stream dataStream = WResp.GetResponseStream())
                {
                    // Open the Stream using a StreamReader for easy access
                    StreamReader reader = new StreamReader(dataStream);

                    // Read the content
                    response = reader.ReadToEnd();
                }
                WResp.Close();

                return response;
            }
            
            string error = ((HttpWebResponse)WResp).StatusDescription;
            ShowError(error, MessageType.Error);

            return null;
        }

        public string GetProjectPath(string binderHTML, bool isBD = false)
        {
            HtmlAgilityPack.HtmlDocument binderDoc = new HtmlAgilityPack.HtmlDocument();

            binderDoc.LoadHtml(binderHTML);

            // First, look to see if there is no link
            string noLinkLookupClass = "xxlGrayTXT";

            HtmlNode noLinkNode = binderDoc.DocumentNode.SelectSingleNode("//*[@class=\"" + noLinkLookupClass + "\"] ");

            if (noLinkNode != null)
            {
                string innerHtml = noLinkNode.InnerHtml;
                string lookupString;
                if (isBD)
                {
                    lookupString = "business";
                } else
                {
                    lookupString = "project";
                }

                if (innerHtml.ToLower().Contains(lookupString))
                {
                    return UNAVAILABLE;
                }
            }

            string lookupClass;
            if (isBD)
            {
                lookupClass = "xxlGreenTXT";
            }
            else
            {
                lookupClass = "xxlOrangeTXT";
            }

            HtmlNode projectFolderNode = binderDoc.DocumentNode.SelectSingleNode("//*[@class=\"" + lookupClass + "\"] ");

            if (projectFolderNode == null)
            {
                LogStatus(string.Concat("Error: Could not find class: ", lookupClass,". Dumping HTML..."));
                LogStatus(binderHTML);

                if (binderHTML.ToLower().Contains("confidential"))
                {
                    return CONFIDENTIAL;
                }

                return null;
            }

            string outerHtml = projectFolderNode.OuterHtml;
            int hrefIndex = outerHtml.IndexOf("href=");
            int startIndex = hrefIndex + "href=\"".Length;
            int endIndex = outerHtml.Substring(startIndex).IndexOf("\"");

            return outerHtml.Substring(startIndex, endIndex);
        }

        private async void ProjectNo_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (!_isMessageBoxShowing)
                {
                    await LoadProjectFolder();
                }
            }
        }

        public Dictionary<string, string[]> LoadJson()
        {
            string json = File.ReadAllText("servers.json");
            return new JavaScriptSerializer().Deserialize<Dictionary<string, string[]>>(json);
        }

        private void LoadStrings()
        {
            string json = File.ReadAllText("strings.json");
            Dictionary<string, string> strings = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json);

            _homeURL = strings["homeURL"];
            _homeHeaderURL = strings["homeHeaderURL"];
            _SIDlookupURL = strings["SIDlookupURL"];
            _binderURL = strings["binderURL"];
        }

        public string CheckJsonForProject(string projectNo)
        {
            if (projectNo == null)
            {
                return null;
            }

            LogStatus("Checking JSON file...");

            string path;
            Dictionary<string, string[]> json = LoadJson();
            if (json.ContainsKey(projectNo))
            {
                LogStatus("Project found in JSON...");

                string[] projectPaths = json[projectNo];
                path = "file:" + projectPaths[0];
                return path;
            }
            else
            {
                LogStatus("Project not found in JSON...");

                return UNAVAILABLE;
            }
        }
    }
}
