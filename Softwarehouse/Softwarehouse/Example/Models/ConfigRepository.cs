using System;
using System.Data.Linq;
using System.Linq;

namespace Quickstem.Models
{
    public class ConfigRepository
    {
        private readonly QuickstemDataContext db;

        public ConfigRepository(DataContext db)
        {
            this.db = (QuickstemDataContext)db;
        }

        public string boxType_url
        {
            get
            {
                return "http://www.floraregistry.com/download.asp?listtype=boxtypes&format=csv";
            }
            set
            {
                setConfig(1, value);
            }
        }

        public string registry_username
        {
            get
            {
                return "sots@floraregistry.com";
            }
            set
            {
                setConfig(2, value);
            }
        }

        public string registry_password
        {
            get
            {
                return "1234";
            }
            set
            {
                setConfig(3, value);
            }
        }

        public string grade_url
        {
            get
            {
                return "http://www.floraregistry.com/download.asp?listtype=grades&format=csv";
            }
            set
            {
                setConfig(4, value);
            }
        }

        public string variety_url
        {
            get
            {
                return "http://www.floraregistry.com/download.asp?listtype=varieties&format=csv";
            }
            set
            {
                setConfig(5, value);
            }
        }

        public bool use_box_numbers
        {
            get
            {
                try
                {
                    return bool.Parse(getConfig(7));
                }
                catch (Exception)
                {
                    return false;
                }
            }
            set
            {
                setConfig(7, value.ToString());
            }
        }

        public bool use_barcodes
        {
            get
            {
                try
                {
                    return bool.Parse(getConfig(8));
                }
                catch (Exception)
                {
                    return false;
                }
            }
            set
            {
                setConfig(8, value.ToString());
            }
        }

        public int registered_grower_id
        {
            get
            {
                return int.Parse(getConfig(10));
            }
            set
            {
                setConfig(10, value.ToString());
            }
        }

        public string smtp_server
        {
            get
            {
                return getConfig(11);
            }
            set
            {
                setConfig(11, value);
            }
        }

        public string smtp_port
        {
            get
            {
                return getConfig(12);
            }
            set
            {
                setConfig(12, value);
            }
        }

        public string smtp_username
        {
            get
            {
                return getConfig(13);
            }
            set
            {
                setConfig(13, value);
            }
        }

        public string smtp_password
        {
            get
            {
                return getConfig(14);
            }
            set
            {
                setConfig(14, value);
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
