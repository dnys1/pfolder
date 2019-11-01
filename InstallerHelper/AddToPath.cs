using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

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
            Registry.LocalMachine.CreateSubKey(keyName).SetValue("Path", oldPath + Context.Parameters["TargetDir"] + ";", RegistryValueKind.ExpandString);
        }
    }
}
