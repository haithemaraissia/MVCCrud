//Generic Mvc Crud Controller to perform all default CRUD actions
//Developed by Matthew Hood - 2009

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace Softwarehouse.MvcCrud.Controllers
{
    public abstract class GenericCrudController<TKeyType, TModel> : Controller where TModel : new()
    {
        //VoidMethod and delegates originally by John Oxley
        protected delegate void VoidMethod<T>(T item);
        protected VoidMethod<TModel> CreateItem;
        protected VoidMethod<TModel> SaveItem;
        protected VoidMethod<TKeyType> DeleteItem;
        protected Func<IEnumerable<TModel>> AllItems;
        protected Func<TKeyType, TModel> SingleItem;

        public virtual ActionResult Index()
        {
            return View("Index", AllItems());
        }

        public virtual ActionResult Create()
        {
            TModel item = new TModel();
            CreateEditSetup(item);
            return View("Create", item);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Create(TModel item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CreateItem(item);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("OTHER", e.Message);
                }
            }

            CreateEditSetup(item);
            return View("Create", item);
        }

        public virtual ActionResult Edit(TKeyType id)
        {
            TModel item = SingleItem(id);
            CreateEditSetup(item);
            return View("Edit", item);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Edit(TModel item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SaveItem(item);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("OTHER", e.Message);
                }
            }

            CreateEditSetup(item);
            return View("Edit", item);
        }

        public virtual ActionResult Delete(TKeyType id)
        {
            try
            {
                DeleteItem(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("OTHER", e.Message);
                return Index();
            }

            return RedirectToAction("Index");
        }

        public virtual ActionResult Details(TKeyType id)
        {
            TModel item = SingleItem(id);
            return View(item);
        }

        protected virtual void CreateEditSetup(TModel item) { }
    }
}
