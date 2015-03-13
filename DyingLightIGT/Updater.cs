using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DyingLightIGT
{
    public static class Updater
    {
        public static Version Check()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/Dalet/DyingLightIGT/releases/latest");
            request.UserAgent = "custom";
            request.Accept = "application/vnd.github.v3+json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            Dictionary<string, dynamic> json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(responseFromServer);
            string version = json["tag_name"];
            if (version != null)
            {
                if (version.StartsWith("v"))
                    version = version.Remove(0, 1);
                return new Version(version);
            }
            return new Version("0.0.0.0");
        }
    }

}
