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
        public void Path_Beginning_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;C:\Program Files\dotnet\;";
            string newPath = AddToPath.getNewPath(currentPath, targetDir);
            Assert.AreEqual(@"C:\Program Files\dotnet\;", newPath);
        }

        [TestMethod()]
        public void Path_Middle_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files\dotnet\;C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;C:\Program Files\Git\cmd;";
            string newPath = AddToPath.getNewPath(currentPath, targetDir);
            Assert.AreEqual(@"C:\Program Files\dotnet\;C:\Program Files\Git\cmd;", newPath);
        }

        [TestMethod()]
        public void Path_End_Test()
        {
            string targetDir = @"C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\";
            string currentPath = @"C:\Program Files\dotnet\;C:\Program Files (x86)\Brown and Caldwell\Project Folder Tool\;";
            string newPath = AddToPath.getNewPath(currentPath, targetDir);
            Assert.AreEqual(@"C:\Program Files\dotnet\;", newPath);
        }
    }
}
