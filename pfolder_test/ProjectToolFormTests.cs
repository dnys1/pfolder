using Microsoft.VisualStudio.TestTools.UnitTesting;
using pfolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;

namespace pfolder.Tests
{
    [TestClass()]
    public class ProjectToolFormTests
    {
        /// <summary>
        /// All the test information is saved in a JSON file to allow easy changing
        /// and maintaining BC project confidentiality. The structure of the JSON can be inferred
        /// from the code below but each object is formatted as follows:
        /// </summary>
        /// <example>
        /// <code>
        /// "Name_Of_Test": {
        ///     "projectNo": "123456",
        ///     "expectedSID": "12345",
        ///     "expectedProjectPath": "file:\\\\server\projects\123456\",
        ///     "expectedBDPath": "file:\\\\server\BD\123456\"
        /// }
        /// </code>
        /// </example>
        private Dictionary<string, Dictionary<string, string>> _tests;

        private void _setupTests()
        {
            if (_tests != null)
            {
                return;
            }

            string json = File.ReadAllText("tests.json");
            _tests = new JavaScriptSerializer().Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
        }

        [TestMethod()]
        public void Cookie_Test()
        {
            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
        }

        [TestMethod()]
        public void NTID_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            string expectedNTID = _tests["NTID_Test"]["expectedNTID"];

            string NTID = AsyncHelpers.RunSync<string>(form.GetNTID);
            Assert.AreEqual(expectedNTID, NTID, "NTID retrieved is invalid.");
        }

        [TestMethod()]
        public void SID_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            string projectNo = _tests["SID_Test"]["projectNo"];
            string expectedSID = _tests["SID_Test"]["expectedSID"];

            string SID = AsyncHelpers.RunSync<string>(() => form.GetSID(projectNo));
            Assert.AreEqual(expectedSID, SID, "SID retrieved is invalid.");
        }

        [TestMethod()]
        public void Project_PDS_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            // Test 1: Non-confidential, PDS project, Phoenix server
            string projectNo = _tests["Project_PDS_Test"]["projectNo"];
            string expectedSID = _tests["Project_PDS_Test"]["expectedSID"];
            string expectedProjectPath = _tests["Project_PDS_Test"]["expectedProjectPath"];
            string expectedBDPath = _tests["Project_PDS_Test"]["expectedBDPath"];

            string SID = AsyncHelpers.RunSync<string>(() => form.GetSID(projectNo));
            Assert.AreEqual(expectedSID, SID, "SID retrieved is invalid for project .");

            string binderHTML = AsyncHelpers.RunSync<string>(() => form.GetBinderHTML(SID));
            Assert.IsNotNull(binderHTML, "Binder HTML retrieved is null.");

            string projectPath = form.GetProjectPath(binderHTML);
            Assert.AreEqual(expectedProjectPath, projectPath, "Project path retrieved is invalid.");

            string BDPath = form.GetProjectPath(binderHTML, isBD: true);
            Assert.AreEqual(expectedBDPath, BDPath, "BD Path retrieved is invalid.");
        }

        [TestMethod()]
        public void Project_Confidential_NoAccess_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            // Test 2: Confidential (no access)
            string projectNo = _tests["Project_Confidential_NoAccess_Test"]["projectNo"];
            string expectedSID = _tests["Project_Confidential_NoAccess_Test"]["expectedSID"];
            string expectedProjectPath = ProjectToolForm.CONFIDENTIAL;
            string expectedBDPath = ProjectToolForm.CONFIDENTIAL;

            string SID = AsyncHelpers.RunSync<string>(() => form.GetSID(projectNo));
            Assert.AreEqual(expectedSID, SID, "SID retrieved is invalid for project .");

            string binderHTML = AsyncHelpers.RunSync<string>(() => form.GetBinderHTML(SID));
            Assert.IsNotNull(binderHTML, "Binder HTML retrieved is null.");

            string projectPath = form.GetProjectPath(binderHTML);
            Assert.AreEqual(expectedProjectPath, projectPath, "Project path retrieved is invalid.");

            string BDPath = form.GetProjectPath(binderHTML, isBD: true);
            Assert.AreEqual(expectedBDPath, BDPath, "BD Path retrieved is invalid.");
        }

        [TestMethod()]
        public void Project_Confidential_Access_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            // Test 3: Confidential (w/ access)
            string projectNo = _tests["Project_Confidential_Access_Test"]["projectNo"];
            string expectedSID = _tests["Project_Confidential_Access_Test"]["expectedSID"];
            string expectedProjectPath = _tests["Project_Confidential_Access_Test"]["expectedProjectPath"];
            string expectedBDPath = _tests["Project_Confidential_Access_Test"]["expectedBDPath"];

            string SID = AsyncHelpers.RunSync<string>(() => form.GetSID(projectNo));
            Assert.AreEqual(expectedSID, SID, "SID retrieved is invalid for project.");

            string binderHTML = AsyncHelpers.RunSync<string>(() => form.GetBinderHTML(SID));
            Assert.IsNotNull(binderHTML, "Binder HTML retrieved is null.");

            string projectPath = form.GetProjectPath(binderHTML);
            Assert.AreEqual(expectedProjectPath, projectPath, "Project path retrieved is invalid.");

            string BDPath = form.GetProjectPath(binderHTML, isBD: true);
            Assert.AreEqual(expectedBDPath, BDPath, "BD Path retrieved is invalid.");
        }

        [TestMethod()]
        public void Project_NonPDS_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            // Test 4: Old project, not in PDS, Phoenix server
            string projectNo = _tests["Project_NonPDS_Test"]["projectNo"];
            string expectedSID = projectNo;
            string expectedProjectPath = _tests["Project_NonPDS_Test"]["expectedProjectPath"];
            string expectedBDPath = ProjectToolForm.UNAVAILABLE;

            string SID = AsyncHelpers.RunSync<string>(() => form.GetSID(projectNo));
            Assert.AreEqual(expectedSID, SID, "SID retrieved is invalid for project .");

            string projectPath = AsyncHelpers.RunSync<string>(() => form.LoadProjectFolder(projectNo));
            Assert.AreEqual(expectedProjectPath, projectPath);

            string BDPath = AsyncHelpers.RunSync<string>(() => form.LoadProjectFolder(projectNo, isBD: true));
            Assert.AreEqual(expectedBDPath, BDPath);
        }

        [TestMethod()]
        public void GoToProfile_Test()
        {
            _setupTests();

            ProjectToolForm form = new ProjectToolForm(testing: true);

            string cookie = AsyncHelpers.RunSync<string>(form.GetCookie);
            Assert.IsNotNull(cookie, "Retrieved cookie is null.");
            form.Cookie = cookie;

            // Test 5: Go to WorkSmart profile test
            string projectNo = _tests["GoToProfile_Test"]["projectNo"];
            string expectedSID = _tests["GoToProfile_Test"]["expectedSID"];
            string expectedProjectPath = _tests["GoToProfile_Test"]["expectedProjectPath"];

            string SID = AsyncHelpers.RunSync<string>(() => form.GetSID(projectNo));
            Assert.AreEqual(expectedSID, SID, "SID retrieved is invalid for project .");

            string projectPath = AsyncHelpers.RunSync<string>(() => form.LoadProjectFolder(projectNo, goToProfile: true));
            Assert.AreEqual(expectedProjectPath, projectPath);
        }
    }
}