using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstallerHelper;

namespace InstallerHelper.Tests
{
    [TestClass()]
    public class InstallerHelperTests
    {
        [TestMethod()]
        public void AddPath_Semicolon_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files\dotnet\;";
            string expectedPath = @"C:\Program Files\dotnet\;C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;";
            string newPath = AddToPath.addToPath(currentPath, targetDir);

            Assert.AreEqual(expectedPath, newPath);
        }

        [TestMethod()]
        public void AddPath_NoSemicolon_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files\dotnet\";
            string expectedPath = @"C:\Program Files\dotnet\;C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;";
            string newPath = AddToPath.addToPath(currentPath, targetDir);

            Assert.AreEqual(expectedPath, newPath);
        }

        [TestMethod()]
        public void RemovePath_Beginning_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;C:\Program Files\dotnet\;";
            string expectedPath = @"C:\Program Files\dotnet\;";
            string newPath = AddToPath.removeFromPath(currentPath, targetDir);

            Assert.AreEqual(expectedPath, newPath);
        }

        [TestMethod()]
        public void RemovePath_Middle_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files\dotnet\;C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;C:\Program Files\Git\cmd;";
            string newPath = AddToPath.removeFromPath(currentPath, targetDir);
            Assert.AreEqual(@"C:\Program Files\dotnet\;C:\Program Files\Git\cmd;", newPath);
        }

        [TestMethod()]
        public void RemovePath_End_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files\dotnet\;C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;";
            string newPath = AddToPath.removeFromPath(currentPath, targetDir);
            Assert.AreEqual(@"C:\Program Files\dotnet\;", newPath);
        }
    }
}
