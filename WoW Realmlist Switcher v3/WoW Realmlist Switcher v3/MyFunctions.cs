using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoW_Realmlist_Switcher_v3.Properties;

namespace WoW_Realmlist_Switcher_v3
{
    class MyFunctions
    {
        public static string GetAppVersion()
        {
            string version = "0";
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(Settings.Default["version_url"].ToString());
                StreamReader reader = new StreamReader(stream);
                string version_data = reader.ReadToEnd();
                reader.Close();
                stream.Close();

                version = version_data.Substring(version_data.IndexOf("Version = ") + "Version = ".Length);
            }
            catch { }
            return version;
        }
    }
}
