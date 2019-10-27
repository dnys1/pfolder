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
            if (args.Length != 0)
            {
                Regex projectNoMatcher = new Regex(@"^\d\d\d\d\d\d$");
                foreach (string arg in args)
                {
                    Console.Write(arg + " ");
                    if (projectNoMatcher.IsMatch(arg))
                    {
                        projectNo = arg;
                        Console.WriteLine(true);
                        break;
                    }
                    Console.WriteLine(false);
                }
            }
            if (projectNo == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ProjectToolForm());
            } else
            {
                AsyncHelpers.RunSync(() => new ProjectToolForm().LoadProjectFolder(projectNo));
            }
        }
    }
}
