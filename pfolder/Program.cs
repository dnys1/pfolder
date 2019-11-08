using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace pfolder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string projectNo = null;
            bool isBD = false;
            bool goToProfile = false;
            if (args.Length != 0)
            {
                Regex projectNoMatcher = new Regex(@"^\d\d\d\d\d\d$");
                foreach (string arg in args)
                {
                    if (projectNoMatcher.IsMatch(arg))
                    {
                        projectNo = arg;
                        continue;
                    }
                    
                    if (arg.ToLower().Contains("b"))
                    {
                        isBD = true;
                        continue;
                    }

                    if (arg.ToLower().Contains("w"))
                    {
                        goToProfile = true;
                        continue;
                    }
                }
            }
            if (projectNo == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ProjectToolForm());
            } else
            {
                AsyncHelpers.RunSync(() => new ProjectToolForm().LoadProjectFolder(projectNo, isBD: isBD, goToProfile: goToProfile));
            }
        }
    }
}
