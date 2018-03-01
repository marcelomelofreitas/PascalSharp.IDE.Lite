using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VisualPascalABCPlugins;
using System.Net;
using System.Reflection;
using Microsoft.Win32;
using PascalSharp.Internal.Localization;

namespace VisualPascalABC
{
    class WorkbenchUpdateService : IWorkbenchUpdateService
    {
        public WorkbenchUpdateService()
        {

        }

        public bool IsDotnet71Installed()
        {
            if (Environment.OSVersion.Version.Major < 6)
                return true;
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
                {
                    return key != null && (key.GetValue("Version") as string).StartsWith("4.7");
                }
            }
            catch (Exception ex)
            {
                return true;
            }
        }
            
        public void CheckForUpdates()
        {
            int status = 1;//1 - up to date, 0 - not up to date, -1 error
            string newVersion = null;
            string curVersion = null;
            if (!IsDotnet71Installed())
            {
                if (MessageBox.Show(StringResources.Get("VP_MF_DOTNET_AVAILABLE"),
                        StringResources.Get("VP_MF_DOTNET_UPDATE_CHECK"),
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    WorkbenchServiceFactory.OperationsService.AddTabWithUrl(".NET Framework", StringResources.Get("VP_MF_FRAMEWORK_DOWNLOAD_PAGE"));
                }
            }
            try
            {
                WebClient client = new WebClient();
                newVersion = client.DownloadString("http://pascalabc.net/downloads/pabcversion.txt").Trim();
                curVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if ((new Version(curVersion)).CompareTo(new Version(newVersion)) == -1)
                    status = 0;
            }
            catch
            {
                status = -1;
            }
            switch (status)
            {
                case 1:
                    MessageBox.Show(StringResources.Get("VP_MF_VERSION_IS_UP_TO_DATE"), StringResources.Get("VP_MF_UPDATE_CHECK"), MessageBoxButtons.OK);
                    break;
                case 0:
                    if (MessageBox.Show(StringResources.Get("VP_MF_UPDATE_AVAILABLE") + Environment.NewLine + 
                        string.Format(StringResources.Get("VP_MF_UPDATE_AVAILABLE_CURRENT_VESION{0}"), curVersion) + Environment.NewLine +
                        string.Format(StringResources.Get("VP_MF_UPDATE_AVAILABLE_NEW_VESION{0}"), newVersion), 
                        StringResources.Get("VP_MF_UPDATE_CHECK"), 
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        WorkbenchServiceFactory.OperationsService.AddTabWithUrl("PascalABC.NET", StringResources.Get("VP_MF_PABC_DOWNLOAD_PAGE"));
                    }
                    break;
                case -1:
                    MessageBox.Show(StringResources.Get("VP_MF_UPDATE_CHECK_ERROR"), StringResources.Get("VP_MF_UPDATE_CHECK"), MessageBoxButtons.OK);
                    break;
            }
        }
    }
}
