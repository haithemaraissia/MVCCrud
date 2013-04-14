using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Example.Models;
//using Softwarehouse.MvcCrud.Updater;

namespace Example.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        /*
        public void Update()
        {
            Updater.Download(MvcApplication.cfg.download_url, "test_", MvcApplication.cfg.update_path, MvcApplication.cfg.app_version, Response);
            MvcApplication.cfg.app_version = Updater.Extract(MvcApplication.cfg.update_path, "test_", MvcApplication.cfg.app_version, Response);
            MvcApplication.cfg.sql_version = Updater.SqlUpdate(MvcApplication.cfg.update_path, "", MvcApplication.cfg.sql_version, Response, new ExampleDataContext());
        }
        */
    }
}
