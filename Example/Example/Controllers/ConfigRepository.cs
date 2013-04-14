using System;
using System.Data.Linq;
using System.Linq;
using Example.Models;

namespace Example.Models
{
    public class ConfigRepository
    {
        private readonly ExampleDataContext db;

        public ConfigRepository(DataContext db)
        {
            this.db = (ExampleDataContext)db;
        }

        public string update_path
        {
            get
            {
                return getConfig(1);
            }
            set
            {
                setConfig(1, value);
            }
        }

        public string download_url
        {
            get
            {
                return getConfig(2);
            }
            set
            {
                setConfig(2, value);
            }
        }

        public int app_version
        {
            get
            {
                return int.Parse(getConfig(3));
            }
            set
            {
                setConfig(3, value.ToString());
            }
        }

        public int sql_version
        {
            get
            {
                return int.Parse(getConfig(4));
            }
            set
            {
                setConfig(4, value.ToString());
            }
        }

        private string getConfig(int id)
        {
            try
            {
                return Enumerable.Single(db.tblConfigs, c => c.id == id).value;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void setConfig(int id, string value)
        {
            if (db.tblConfigs.Count(c => c.id == id) == 1)
            {
                db.tblConfigs.Single(c => c.id == id).value = value;
                db.SubmitChanges();
            }
            else
            {
                tblConfig c = new tblConfig { id = id, value = value };
                db.tblConfigs.InsertOnSubmit(c);
                db.SubmitChanges();
            }
        }
    }
}
