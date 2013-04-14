//Removed as requires Chillkat dll and valid license

/* 
using System;
using System.Data.Linq;
using System.IO;
using System.Net;
using System.Web;
using Chilkat;

namespace Softwarehouse.MvcCrud.Updater
{
    public class Updater
    {
        public static void Download(string dl_url, string prefix, string save_path, int current_verion, HttpResponseBase response)
        {
            WebClient webClient = new WebClient();
            int i = current_verion + 1;

            //Get latest downloads only, no need to get ones that are already there
            while (File.Exists(Path.Combine(Path.Combine(save_path, "Updates"), prefix + i + ".zip")))
            {
                i++;
            }

            response.Write("Downloading updates after : " + prefix + (i - 1) + ".zip<br />");
            response.Flush();

            //Download all new updates
            while (true)
            {
                try
                {
                    webClient.DownloadFile(Path.Combine(dl_url, prefix + i + ".zip"),
                                           Path.Combine(Path.Combine(save_path, "Updates"), prefix + i + ".zip"));
                    response.Write("Downloaded update : " + prefix + i + ".zip<br />");
                    response.Flush();
                    i++;
                    
                }
                catch(Exception)
                {
                    response.Write("No further updates available for download.<br />");
                    response.Flush();
                    break;
                }
            }
        }

        public static int Extract(string save_path, string prefix, int current_version, HttpResponseBase response)
        {
            int i = current_version + 1;

            while (File.Exists(Path.Combine(Path.Combine(save_path, "Updates"), prefix + i + ".zip")))
            {
                try
                {
                    response.Write("Uncompressing " + prefix + i + ".zip<br />");
                    response.Flush();
                    
                    Chilkat.Zip zip = new Zip();
                    //Insert your chillkat license here to use the updater
                    zip.UnlockComponent("");
                    zip.OpenZip(Path.Combine(Path.Combine(save_path, "Updates"), prefix + i + ".zip"));

                    zip.Extract(save_path);

                    current_version = i;
                    i++;
                }
                catch (Exception e)
                {
                    response.Write("EXTRACTION ERROR: " + e.Message + "<br />");
                    response.Flush();
                    break;
                }
            }

            response.Write("File update complete.<br />");
            response.Flush();

            return current_version;
        }

        public static int SqlUpdate(string save_path, string prefix, int sql_version, HttpResponseBase response, DataContext db)
        {
            int i = sql_version + 1;

            while (File.Exists(Path.Combine(Path.Combine(save_path, "Updates\\SQL"), prefix + i + ".sql")))
            {
                try
                {
                    response.Write("Running db script " + prefix + i + ".sql<br />");
                    response.Flush();

                    string sql = File.ReadAllText(Path.Combine(Path.Combine(save_path, "Updates\\SQL"), prefix + i + ".sql"));

                    db.ExecuteCommand(sql);

                    sql_version = i;
                    i++;
                }
                catch (Exception e)
                {
                    response.Write("DB ERROR: " + e.Message + "<br />");
                    response.Flush();
                    break;
                }
            }

            response.Write("Database update complete.<br />");
            response.Flush();

            return sql_version;
        }
    }
}
 */
