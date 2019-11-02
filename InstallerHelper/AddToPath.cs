using Microsoft.Win32;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace InstallerHelper
{
    [RunInstaller(true)]
    public partial class AddToPath : System.Configuration.Install.Installer
    {
        public AddToPath()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            string keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            // Get non-expanded PATH environment variable 
            string oldPath = (string)Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
            // Include target directory in PATH variable
            string targetDir = Context.Parameters["TargetDir"];
            if (oldPath.IndexOf(targetDir) != -1)
            {
                return;
            }

            string separator = "";
            if (oldPath.ElementAt(oldPath.Length - 1) != ';')
            {
                separator = ";";
            }
            string newPath = oldPath + separator + targetDir + ";";
            
            Registry.LocalMachine.CreateSubKey(keyName).SetValue("Path", newPath, RegistryValueKind.ExpandString);
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);

            string keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            // Get non-expanded PATH environment variable 
            string currentPath = (string)Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
            // Remove target directory from PATH variable
            string targetDir = Context.Parameters["TargetDir"];
            string newPath = getNewPath(currentPath, targetDir);

            Registry.LocalMachine.CreateSubKey(keyName).SetValue("Path", newPath, RegistryValueKind.ExpandString);
        }

        public static string getNewPath(string currentPath, string targetDir)
        {
            int startIndex = currentPath.IndexOf(targetDir);
            if (startIndex == 0)
            {
                return currentPath.Substring(targetDir.Length + 1);
            } else
            {
                return currentPath.Substring(0, startIndex - 1) + currentPath.Substring(startIndex + targetDir.Length);
            }
        }
    }
}
