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
            string currentPath = (string)Registry.LocalMachine.CreateSubKey(keyName).GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
            // Include target directory in PATH variable
            string targetDir = Context.Parameters["TargetDir"];
            if (currentPath.IndexOf(targetDir) != -1)
            {
                return;
            }

            string newPath = addToPath(currentPath, targetDir);
            
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
            string newPath = removeFromPath(currentPath, targetDir);

            Registry.LocalMachine.CreateSubKey(keyName).SetValue("Path", newPath, RegistryValueKind.ExpandString);
        }

        public static string addToPath(string currentPath, string targetDir)
        {
            string separator = "";
            if (currentPath.ElementAt(currentPath.Length - 1) != ';')
            {
                separator = ";";
            }
            return currentPath + separator + targetDir + ";";
        }

        public static string removeFromPath(string currentPath, string targetDir)
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
